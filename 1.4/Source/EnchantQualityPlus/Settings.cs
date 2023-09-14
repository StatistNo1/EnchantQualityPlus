using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnchantQualityPlus {
    public class Settings : Verse.ModSettings {
        public static bool ImportantMessage20230915 = false;
        public static string ChoiceOfMechanic = "ScalingSquished";
        public List<string> ChoicesOfMechanic = new() { "ScalingSquished", "ScalingAdded", "LegacyLegendary", "LegacyMasterwork", "LegacyExcellent" };
        public override void ExposeData() {
            Verse.Scribe_Values.Look(ref ImportantMessage20230915, "ImportantMessage20230915");
            Verse.Scribe_Values.Look(ref ChoiceOfMechanic, "ChoiceOfMode");
            base.ExposeData();
        }
    }

    public class EvolvedOrgansReduxSettings : Verse.Mod {
        readonly Settings settings;
        public EvolvedOrgansReduxSettings(Verse.ModContentPack content) : base(content) {
            this.settings = GetSettings<Settings>();
        }
        public override void DoSettingsWindowContents(UnityEngine.Rect inRect) {
            Verse.Listing_Standard listingStandard = new Verse.Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("Which mode do you want to use?");
            listingStandard.Label("ScalingSquished squishes the scaling down and unlocks the Legendary enchant with 250% psychic sensitivity.");
            listingStandard.Label("ScalingAdded keeps the base scaling and just adds the Legendary enchant unlock at 350% psychich sensitivity.");
            listingStandard.Label("LegacyLegendary has the same mechanic as it used to have. Items can be enchanted until Legendary.");
            listingStandard.Label("LegacyLegendary has the same mechanic as it used to have. Items can be enchanted until Masterwork.");
            listingStandard.Label("LegacyLegendary has the same mechanic as it used to have. Items can be enchanted until Excellent.");
            listingStandard.Gap();
            for (int i = 0; i < settings.ChoicesOfMechanic.Count; i++) {
                if (listingStandard.RadioButton(settings.ChoicesOfMechanic[i], Settings.ChoiceOfMechanic == settings.ChoicesOfMechanic[i], tabIn: 30f)) {
                    Settings.ChoiceOfMechanic = settings.ChoicesOfMechanic[i];
                }
            }
            listingStandard.End();
        }
        public override string SettingsCategory() {
            return "EnchantQualityPlus";
        }
    }
}
