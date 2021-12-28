namespace IcyPhoenix.WorldFlipper.Extractor.Header
{
    public class AbilityHeader
    {
        private static readonly string[] HeaderStrings = new[]
        {
            "ID",
            "String_ID",
            "Unisonable",
            "Statue_Group_ID",
            "AbilityTriggerMasterValue",
            "AbilityPreconditionMasterValue",
            "AbilityTriggerPullerMasterValue",
            "AbilityTriggerPullerMasterValue_character_groups",
            "AbilityPreconditionMasterValue_threshold_Power1",
            "AbilityPreconditionMasterValue_threshold_FirstMax",
            "AbilityPreconditionMasterValue_character_groups",
            "InstantAbilityTriggerMasterValue",
            "AbilityTriggerPullerMasterValue",
            "AbilityTriggerPullerMasterValue_character_groups",
            "InstantAbilityTriggerMasterValue_threshold_power1",
            "InstantAbilityTriggerMasterValue_threshold_first_max",
            "InstantAbilityTriggerMasterValue_trigger_limit",
            "AbilityTriggerPullerMasterValue_character_groups",
            "InstantAbilityContentMasterValue",
            "AbilityTargetMasterValue",
            "AbilityTargetMasterValue_character_groups",
            "InstantAbilityContentMasterValue_strength_power1",
            "InstantAbilityContentMasterValue_strength_first_max",
            "InstantAbilityContentMasterValue_frame_power1",
            "InstantAbilityContentMasterValue_frame_first_max",
            "InstantAbilityContentMasterValue_max_accumulation",
            "InstantAbilityContentMasterValue_flip_limit",
            "Threshold_FirstMax",
            "DuringAbilityTriggerMasterValue",
            "AbilityTriggerPullerMasterValue",
            "AbilityTriggerPullerMasterValue_character_groups",
            "DuringAbilityTriggerMasterValue_threshold_power1",
            "DuringAbilityTriggerMasterValue_threshold_first_max",
            "DuringAbilityTriggerMasterValue_trigger_limit",
            "CommonAbilityContentMasterValue",
            "AbilityTargetMasterValue",
            "AbilityTargetMasterValue_character_groups",
            "CommonAbilityContentMasterValue_strength_power1",
            "CommonAbilityContentMasterValue_strength_first_max",
            "CommonAbilityContentMasterValue_character_groups",
            "OpeningAbilityMasterValue",
            "OpeningAbilityMasterValue_strength_power1",
            "OpeningAbilityMasterValue_strength_first_max",
        };

        private const string abilityJosnLocation = @".\master\ability\ability.json";

        
    }

    public class Ability
    {
        public string ID { get; set; }
        public string String_ID { get; set; }
        public string Unisonable { get; set; }
        public string Statue_Group_ID { get; set; }
        public string AbilityTriggerMasterValue { get; set; }
        public string AbilityPreconditionMasterValue { get; set; }
        public string AbilityTriggerPullerMasterValue { get; set; }
        public string AbilityTriggerPullerMasterValue_character_groups { get; set; }
        public string AbilityPreconditionMasterValue_threshold_Power1 { get; set; }
        public string AbilityPreconditionMasterValue_threshold_FirstMax { get; set; }
        public string AbilityPreconditionMasterValue_character_groups { get; set; }
        public string InstantAbilityTriggerMasterValue { get; set; }
        public string AbilityTriggerPullerKind { get; set; }
        public string AbilityTriggerPullerKind_character_groups { get; set; }
        public string InstantAbilityTriggerMasterValue_threshold_power1 { get; set; }
        public string InstantAbilityTriggerMasterValue_threshold_first_max { get; set; }
        public string InstantAbilityTriggerMasterValue_trigger_limit { get; set; }
        public string InstantAbilityTriggerMasterValue_character_group { get; set; }
        public string InstantAbilityContentMasterValue { get; set; }
        public string AbilityTargetMasterValue { get; set; }
        public string AbilityTargetMasterValue_character_groups { get; set; }
        public string InstantAbilityContentMasterValue_strength_power1 { get; set; }
        public string InstantAbilityContentMasterValue_strength_first_max { get; set; }
        public string InstantAbilityContentMasterValue_frame_power1 { get; set; }
        public string InstantAbilityContentMasterValue_frame_first_max { get; set; }
        public string InstantAbilityContentMasterValue_max_accumulation { get; set; }
        public string InstantAbilityContentMasterValue_flip_limit { get; set; }
        public string Threshold_FirstMax { get; set; }
        public string DuringAbilityTriggerMasterValue { get; set; }
        public string AbilityTriggerPullerKind2 { get; set; }
        public string AbilityTriggerPullerMasterValue_character_groups2 { get; set; }
        public string DuringAbilityTriggerMasterValue_threshold_power1 { get; set; }
        public string DuringAbilityTriggerMasterValue_threshold_first_max { get; set; }
        public string DuringAbilityTriggerMasterValue_trigger_limit { get; set; }
        public string CommonAbilityContentMasterValue { get; set; }
        public string AbilityTargetMasterValue2 { get; set; }
        public string AbilityTargetMasterValue_character_groups2 { get; set; }
        public string CommonAbilityContentMasterValue_strength_power1 { get; set; }
        public string CommonAbilityContentMasterValue_strength_first_max { get; set; }
        public string CommonAbilityContentMasterValue_character_groups { get; set; }
        public string OpeningAbilityMasterValue { get; set; }
        public string OpeningAbilityMasterValue_strength_power1 { get; set; }
        public string OpeningAbilityMasterValue_strength_first_max { get; set; }
    }
}
