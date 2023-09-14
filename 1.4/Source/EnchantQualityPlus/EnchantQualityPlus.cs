using RimWorld;
using RimWorld.Planet;
using Verse;
using VFECore.Abilities;
using Ability = VFECore.Abilities.Ability;

namespace EnchantQualityPlus.Technomancer {

    [RimWorld.DefOf]
    public static class DefOf {
        public static VFECore.Abilities.AbilityDef VPE_EnchantQuality;
        static DefOf() {
            RimWorld.DefOfHelper.EnsureInitializedInCtor(typeof(RimWorld.AbilityDefOf));
        }
    }
    public class GameComponent : Verse.GameComponent {
        Game game = null;
        public GameComponent(Verse.Game game) { this.game = game; }
        public override void LoadedGame() {
            base.LoadedGame();
            foreach (Map m in game.Maps) {
                foreach (Pawn p in m.mapPawns.AllPawns) {
                    if (p.RaceProps.Humanlike == true && p.HasPsylink) {
                        System.Collections.Generic.List<Ability> list = p.GetComp<CompAbilities>().LearnedAbilities;
                        Ability a = p.GetComp<CompAbilities>().LearnedAbilities.Find(x => x is VanillaPsycastsExpanded.Technomancer.Ability_IncreaseQuality);
                        if (a != null) {
                            list.Remove(a);
                            p.GetComp<CompAbilities>().GiveAbility(a.def);
                        }
                    }
                }
            }
        }
        public override void FinalizeInit() {
            base.FinalizeInit();
            if (!Settings.ImportantMessage20230915) {
                Verse.Find.LetterStack.ReceiveLetter("EnchantQualityPlus info",
                    "Vanilla Psycast Expanded made an update to the Enchant Quality psycast, which means I had to do one as well.\n" +
                    "The updated psycasts maximum quality is now dependant on your casters psychic sensitivity.\n" +
                    "There is a setting that lets you decide how to handle the increased maximum quality of this mod.\n" +
                    "Don't like the scaling mechanic? There is a setting to go back to the old Enchant Quality psycast as well!\n",
                    LetterDefOf.NeutralEvent);
                Settings.ImportantMessage20230915 = true;
                Verse.LoadedModManager.GetMod<EvolvedOrgansReduxSettings>().WriteSettings();

            }
        }
    }
    public class Ability_IncreaseQuality : Ability {
        public override void Cast(params GlobalTargetInfo[] targets) {
            base.Cast(targets);

            foreach (GlobalTargetInfo target in targets) {
                CompQuality comp = target.Thing.GetInnerIfMinified().TryGetComp<CompQuality>();
                if (comp == null || comp.Quality >= MaxQuality) return;
                comp.SetQuality(comp.Quality + 1, ArtGenerationContext.Colony);
                for (int i = 0; i < 16; i++) FleckMaker.ThrowMicroSparks(target.Thing.TrueCenter(), this.pawn.Map);
            }
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true) {
            if (!base.ValidateTarget(target, showMessages)) return false;
            CompQuality comp;
            if ((comp = target.Thing.GetInnerIfMinified().TryGetComp<CompQuality>()) == null) {
                if (showMessages) Messages.Message("VPE.MustHaveQuality".Translate(), MessageTypeDefOf.RejectInput, false);
                return false;
            }
            if (comp.Quality >= MaxQuality) {
                if (showMessages) Messages.Message("VPE.QualityTooHigh".Translate(MaxQuality.GetLabel()), MessageTypeDefOf.RejectInput, false);
                return false;
            }

            return true;
        }

        private QualityCategory MaxQuality {
            get {
                if (Settings.ChoiceOfMechanic == "LegacyExcellent")
                    return QualityCategory.Excellent;
                else if (Settings.ChoiceOfMechanic == "LegacyMasterwork")
                    return QualityCategory.Masterwork;
                else if (Settings.ChoiceOfMechanic == "LegacyLegendary")
                    return QualityCategory.Legendary;
                else return (QualityCategory)(int)GetPowerForPawn();
            }
        }

        public override string GetPowerForPawnDescription() => "VPE.MaxQuality".Translate(MaxQuality.GetLabel()).Colorize(UnityEngine.Color.cyan);
        public override float GetPowerForPawn() {
            if (Settings.ChoiceOfMechanic == "ScalingSquished")
                return pawn.GetStatValue(StatDefOf.PsychicSensitivity) switch {
                    <= 1.2f => (int)QualityCategory.Good,
                    <= 1.5f => (int)QualityCategory.Excellent,
                    <= 2.0f => (int)QualityCategory.Masterwork,
                    > 2.5f => (int)QualityCategory.Legendary,
                    _ => (int)QualityCategory.Normal
                };
            else
                return pawn.GetStatValue(StatDefOf.PsychicSensitivity) switch {
                    <= 1.2f => (int)QualityCategory.Good,
                    <= 2f => (int)QualityCategory.Excellent,
                    <= 2.5f => (int)QualityCategory.Masterwork,
                    > 3.5f => (int)QualityCategory.Legendary,
                    _ => (int)QualityCategory.Normal
                };
        }
    }
}