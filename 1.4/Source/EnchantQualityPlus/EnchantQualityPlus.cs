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
        public GameComponent(Verse.Game game) {this.game = game;}
        public override void LoadedGame() {
            base.LoadedGame();
            foreach (Map m in game.Maps) {
                foreach (Pawn p in m.mapPawns.AllPawns) {
                    if (p.RaceProps.Humanlike == true && p.HasPsylink) {
                        System.Collections.Generic.List<Ability> list = p.GetComp<CompAbilities>().LearnedAbilities;
                        Ability a = p.GetComp<CompAbilities>().LearnedAbilities.Find(x => x is VanillaPsycastsExpanded.Technomancer.Ability_IncreaseQuality);
                        list.Remove(a);
                        p.GetComp<CompAbilities>().GiveAbility(a.def);
                    }
                }
            }
        }
    }
        public class Ability_IncreaseQuality : Ability {
        public override void Cast(params GlobalTargetInfo[] targets) {
            base.Cast(targets);
            foreach (GlobalTargetInfo target in targets) {
                CompQuality comp = target.Thing.GetInnerIfMinified().TryGetComp<CompQuality>();
                if (comp is not { Quality: < QualityCategory.Legendary }) return;
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

            if (comp.Quality >= QualityCategory.Legendary) {
                if (showMessages) Messages.Message("VPE.QualityTooHigh".Translate(), MessageTypeDefOf.RejectInput, false);
                return false;
            }

            return true;
        }
    }
}