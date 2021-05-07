﻿using KaosesTweaks.Utils;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace KaosesTweaks.Models
{
    public class KaosesCharacterDevelopmentModel : DefaultCharacterDevelopmentModel
    {

        // Token: 0x17000B0F RID: 2831
        // (get) Token: 0x06002BD0 RID: 11216 RVA: 0x000A858F File Offset: 0x000A678F
        public override int LevelsPerAttributePoint
        {
            get
            {
                if (Statics._settings.CharacterLevelsPerAttributeModifiers)
                {
                    return Statics._settings.CharacterLevelsPerAttributeValue;
                }
                return 4;
            }
        }

        // Token: 0x17000B10 RID: 2832
        // (get) Token: 0x06002BD1 RID: 11217 RVA: 0x000A8592 File Offset: 0x000A6792
        public override int FocusPointsPerLevel
        {
            get
            {
                if (Statics._settings.CharacterFocusPerLevelModifiers)
                {
                    return Statics._settings.CharacterFocusPerLevelValue;
                }
                return 1;
            }
        }
        /*
         TODO have multiplier for these
         */

        // Token: 0x06002BD7 RID: 11223 RVA: 0x000A8638 File Offset: 0x000A6838
        public override float CalculateLearningRate(Hero hero, SkillObject skill)
        {
            int level = hero.Level;
            int attributeValue = hero.GetAttributeValue(skill.CharacterAttributeEnum);
            int focus = hero.HeroDeveloper.GetFocus(skill);
            int skillValue = hero.GetSkillValue(skill);
            float LearningRate = CalculateLearningRate(attributeValue, focus, skillValue, level, skill.CharacterAttribute.Name, false).ResultNumber;
            return LearningRate;
        }


        // Token: 0x06002BD8 RID: 11224 RVA: 0x000A8690 File Offset: 0x000A6890
        public override ExplainedNumber CalculateLearningRate(int attributeValue, int focusValue, int skillValue, int characterLevel, TextObject attributeName, bool includeDescriptions = false)
        {
            float learningMultiplier = 1.0f;

            if (Statics._settings.LearningRateEnabled)
            {
                learningMultiplier = Statics._settings.LearningRateMultiplier;
            }

            float baseNo = 20f / (10f + (float)characterLevel);
            float baseNumber = baseNo * learningMultiplier;
            ExplainedNumber result = new ExplainedNumber(baseNumber, true, null);

            float att1 = (0.4f * (float)attributeValue);
            float attAdded = ((float)Math.Round((double)(att1), 3) * 100f);
            double attFac = Math.Round((baseNumber * attAdded * 0.01f), 3);
            if (Statics._settings.LearningRateEnabled)
            {
                result.Add((float)(attFac), new TextObject("KT " + attributeName.ToString(), null));
            }
            else
            {
                result.AddFactor(((0.4f * (float)attributeValue)), attributeName);
            }
    
            float fv1 = (float)focusValue * 1f;
            float focusAdded = ((float)Math.Round((double)fv1, 3) * 100f);
            double focusFac = Math.Round((baseNumber * focusAdded * 0.01f), 3);
            if (Statics._settings.LearningRateEnabled)
            {
                result.Add((float)(focusFac), new TextObject("KT " + _skillFocusText, null));
            }
            else
            {
                result.AddFactor(((float)focusValue * 1f), _skillFocusText);
            }
            

            int num = MBMath.Round(this.CalculateLearningLimit(attributeValue, focusValue, null, false).ResultNumber);
            float test = 0.0f;
            int num2 = 0;
            if (skillValue > num)
            {
                num2 = skillValue - num;
                result.AddFactor(-1f - 0.1f * (float)num2, _overLimitText);
                test = -1f - 0.1f * (float)num2;
            }


            result.LimitMin(0f);
            return result;
        }


        #region Required Source Code
        // Token: 0x04000EA0 RID: 3744

        // Token: 0x04000EA1 RID: 3745
        private static TextObject _skillFocusText = new TextObject("{=MRktqZwu}Skill Focus", null);

        // Token: 0x04000EA2 RID: 3746
        private static TextObject _overLimitText = new TextObject("{=bcA7ZuyO}Learning Limit Exceeded", null);
 
        // Token: 0x04000E8D RID: 3725
        private const int MaxCharacterLevels = 62;

        // Token: 0x04000E8E RID: 3726
        private const int MaxAttributeLevel = 11;

        // Token: 0x04000E8F RID: 3727
        private const int SkillPointsAtLevel1 = 1;

        // Token: 0x04000E90 RID: 3728
        private const int SkillPointsGainNeededInitialValue = 1000;

        // Token: 0x04000E91 RID: 3729
        private const int SkillPointsGainNeededIncreasePerLevel = 1000;

        // Token: 0x04000E92 RID: 3730
        private readonly int[] _skillsRequiredForLevel = new int[63];

        // Token: 0x04000E93 RID: 3731
        private const int FocusPointsPerLevelConst = 1;

        // Token: 0x04000E94 RID: 3732
        private const int LevelsPerAttributePointConst = 4;

        // Token: 0x04000E95 RID: 3733
        private const int FocusPointCostToOpenSkillConst = 0;

        // Token: 0x04000E96 RID: 3734
        private const int FocusPointsAtStartConst = 5;

        // Token: 0x04000E97 RID: 3735
        private const int AttributePointsAtStartConst = 15;

        // Token: 0x04000E98 RID: 3736
        private const int MaxSkillLevels = 1024;

        // Token: 0x04000E99 RID: 3737
        private readonly int[] _xpRequiredForSkillLevel = new int[1024];

        // Token: 0x04000E9A RID: 3738
        private const int XpRequirementForFirstLevel = 30;

        // Token: 0x04000E9B RID: 3739
        private const int MaxSkillPoint = 2147483647;

        // Token: 0x04000E9C RID: 3740
        private const int traitThreshold1 = 1000;

        // Token: 0x04000E9D RID: 3741
        private const int traitThreshold2 = 4000;

        // Token: 0x04000E9E RID: 3742
        private const int traitMaxValue1 = 2500;

        // Token: 0x04000E9F RID: 3743
        private const int traitMaxValue2 = 6000;

        // Token: 0x04000EA0 RID: 3744
        #endregion





    }


}


