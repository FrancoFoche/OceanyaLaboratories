using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Storage of all player characters
/// </summary>
public class DBPlayerCharacter : MonoBehaviour
{
    public static List<PlayerCharacter> pCharacters = new List<PlayerCharacter>();


    /// <summary>
    /// Builds the initial database to start pulling information from.
    /// </summary>
    public static void BuildDatabase()
    {
        pCharacters = new List<PlayerCharacter>()
        {

            new PlayerCharacter(0 , "TestDummy" , 1, GameAssetsManager.instance.GetClass(0),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 100 },
                    { Stats.CURHP       , 100 },
                    { Stats.STR      , 100 },
                    { Stats.INT      , 100  },
                    { Stats.CHR      , 100 },
                    { Stats.AGI      , 100 },
                    { Stats.MR       , 100 },
                    { Stats.PR       , 100 },
                    { Stats.CON      , 100  },
                    { Stats.HPREGEN  , 100  }
                },
                GameAssetsManager.instance.GetClass(0).skillList,
                new List<Item>
                {
                }
            ),

            new PlayerCharacter(13 , "Vinnie" , 1,  GameAssetsManager.instance.GetClass(ClassNames.Vampire.ToString()) ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 47 },
                    { Stats.CURHP       , 47 },
                    { Stats.STR      , 10 },
                    { Stats.INT      , 20  },
                    { Stats.CHR      , 30 },
                    { Stats.AGI      , 35 },
                    { Stats.MR       , 16 },
                    { Stats.PR       , 15 },
                    { Stats.CON      , 9  },
                    { Stats.HPREGEN  , 0  }
                },

                new List<Skill>()
                {
                    GameAssetsManager.instance.GetSkill(ClassNames.Vampire.ToString(),"Vampire Fangs"),
                    GameAssetsManager.instance.GetSkill(ClassNames.Vampire.ToString(),"Bat Swarm"),
                    GameAssetsManager.instance.GetSkill(ClassNames.Vampire.ToString(),"Dry their blood"),
                    GameAssetsManager.instance.GetSkill(ClassNames.Doctor.ToString(),"Heal"),
                },
                new List<Item>
                {

                }
            ),
            new PlayerCharacter(5 , "Da Docta" , 9,  GameAssetsManager.instance.GetClass(ClassNames.Doctor.ToString()),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 83    },
                    { Stats.CURHP       , 83    },
                    { Stats.STR         , 1     },
                    { Stats.INT         , 57    },
                    { Stats.CHR         , 9     },
                    { Stats.AGI         , 37    },
                    { Stats.MR          , 2     },
                    { Stats.PR          , 3     },
                    { Stats.CON         , 13    },
                    { Stats.HPREGEN     , 0     }
                },
                new List<Skill>()
                {
                    GameAssetsManager.instance.GetSkill(ClassNames.Doctor.ToString(),"Chakra Scalples"),
                    GameAssetsManager.instance.GetSkill(ClassNames.Doctor.ToString(),"Revival Ritual"),
                    GameAssetsManager.instance.GetSkill(ClassNames.Doctor.ToString(),"Heal"),
                    GameAssetsManager.instance.GetSkill("Testing Class","Regenerative Meditation"),
                },
                new List<Item>()
            ),

            new PlayerCharacter(9 , "Archive" , 5,  GameAssetsManager.instance.GetClass(ClassNames.MasterOfDarkArts.ToString()) ,
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 50    },
                    { Stats.CURHP       , 50    },
                    { Stats.STR         , 1     },
                    { Stats.INT         , 36    },
                    { Stats.CHR         , 2     },
                    { Stats.AGI         , 30    },
                    { Stats.MR          , 10    },
                    { Stats.PR          , 5     },
                    { Stats.CON         , 6     },
                    { Stats.HPREGEN     , 0     }
                },
                new List<Skill>()
                {

                    GameAssetsManager.instance.GetSkill(ClassNames.MasterOfDarkArts.ToString(),"Mind Over Body"),
                    GameAssetsManager.instance.GetSkill(SenjutsuSubclasses.FrogStyleSage.ToString(),"Triple Threat"),
                    GameAssetsManager.instance.GetSkill(ClassNames.MasterOfDarkArts.ToString(),"White Dragon Breath"),
                    GameAssetsManager.instance.GetSkill(ClassNames.MasterOfDarkArts.ToString(),"Soul Spear"),
                    GameAssetsManager.instance.GetSkill("Testing Class","Arcane Overflow"),
                },
                new List<Item>()
            ),
        };
    }

    #region CustomEditor
#if UNITY_EDITOR

    [CustomEditor(typeof(DBPlayerCharacter))]
    public class DBPlayerCharacterEditor : Editor
    {
        DBPlayerCharacter Target;
        private void OnEnable()
        {
            Target = target as DBPlayerCharacter;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Create Enemy Assets"))
            {
                DBEnemies.BuildDatabase();
                for (int i = 0; i < DBEnemies.enemies.Count; i++)
                {
                    Enemy current = DBEnemies.enemies[i];
                    string folder = "Assets/ScriptableObjects/Characters/Enemies";
                    string fileName = $"{current.ID}-{current.name}";
                    string path = folder + "/" + fileName + ".asset";
                    AssetDatabase.CreateAsset(current, path);
                }
            }

            if (GUILayout.Button("Create Player Character Assets"))
            {
                BuildDatabase();
                for (int i = 0; i < pCharacters.Count; i++)
                {
                    PlayerCharacter current = pCharacters[i];
                    string folder = "Assets/ScriptableObjects/Characters/PlayerCharacters";
                    string fileName = $"{current.ID}-{current.name}";
                    string path = folder + "/" + fileName + ".asset";
                    AssetDatabase.CreateAsset(current, path);
                }
            }

            if (GUILayout.Button("Create Skill and Class Assets"))
            {
                DBClasses.BuildDatabase();
                GameAssetsManager.instance.classes = new BaseSkillClass[DBClasses.classes.Count];
                for (int i = 0; i < DBClasses.classes.Count; i++)
                {
                    BaseSkillClass currentClass = DBClasses.classes[i];
                    string classFolder = "Assets/ScriptableObjects/Classes/";
                    string classfileName = $"{currentClass.ID}-{currentClass.name}";
                    string classpath = classFolder + classfileName + ".asset";

                    string subfolder = "Assets/ScriptableObjects/Skills/" + classfileName;

                    if (!AssetDatabase.IsValidFolder(subfolder))
                    {
                        AssetDatabase.CreateFolder("Assets/ScriptableObjects/Skills", classfileName);
                    }

                    for (int j = 0; j < currentClass.skillList.Count; j++)
                    {
                        Skill curSkill = currentClass.skillList[j];
                        curSkill.skillClass = currentClass;
                        string skillfileName = $"{curSkill.ID}-{curSkill.name}";
                        string subpath = subfolder + "/" + skillfileName + ".asset";

                        #region Activation Requirements
                        if (curSkill.activationRequirements != null)
                        {
                            string requirementFolder = "Assets/ScriptableObjects/Rules/ActivationRequirement/";
                            for (int k = 0; k < curSkill.activationRequirements.Count; k++)
                            {
                                ActivationRequirement curRequirement = curSkill.activationRequirements[k];
                                string fileName3 = curRequirement.type != ActivationRequirement.RequirementType.SkillIsActive ? ActivationRequirement.ActivationRequirementEditor.GetActivationRequirementFileName(curRequirement) : $"SkillIsActive {k}";
                                string requirementPath = requirementFolder + fileName3 + ".asset";

                                AssetDatabase.CreateAsset(curRequirement, requirementPath);

                                curSkill.activationRequirements[k] = curRequirement;
                            }
                        }
                        #endregion

                        #region SkillFormulas
                        string formulaFolder = "Assets/ScriptableObjects/Rules/Formulas/";
                        if (curSkill.damageFormula != null)
                        {
                            for (int k = 0; k < curSkill.damageFormula.Count; k++)
                            {
                                RPGFormula curFormula = curSkill.damageFormula[k];
                                string fileName3 = RPGFormula.RPGFormulaCustomEditor.GetRPGFormulaFileName(curFormula);
                                string formulaPath = formulaFolder + fileName3 + ".asset";

                                try
                                {
                                    AssetDatabase.CreateAsset(curFormula, formulaPath);
                                    curSkill.damageFormula[k] = curFormula;
                                }
                                catch
                                {
                                    curSkill.damageFormula[k] = AssetDatabase.LoadMainAssetAtPath(formulaPath) as RPGFormula;
                                }
                            }
                        }
                        if (curSkill.healFormula != null)
                        {
                            for (int k = 0; k < curSkill.healFormula.Count; k++)
                            {
                                RPGFormula curFormula = curSkill.healFormula[k];
                                string fileName3 = RPGFormula.RPGFormulaCustomEditor.GetRPGFormulaFileName(curFormula);
                                string formulaPath = formulaFolder + fileName3 + ".asset";
                                try
                                {
                                    AssetDatabase.CreateAsset(curFormula, formulaPath);
                                    curSkill.healFormula[k] = curFormula;
                                }
                                catch
                                {
                                    curSkill.healFormula[k] = AssetDatabase.LoadMainAssetAtPath(formulaPath) as RPGFormula;
                                }
                            }
                        }
                        if (curSkill.formulaStatModifiers != null)
                        {
                            RuleManager.BuildHelpers();
                            for (int k = 0; k < RuleManager.StatHelper.Length; k++)
                            {
                                Stats curStat = RuleManager.StatHelper[i];

                                if (curSkill.formulaStatModifiers.ContainsKey(curStat))
                                {
                                    for (int m = 0; m < curSkill.formulaStatModifiers[curStat].Count; m++)
                                    {
                                        RPGFormula curFormula = curSkill.formulaStatModifiers[curStat][m];
                                        string fileName3 = RPGFormula.RPGFormulaCustomEditor.GetRPGFormulaFileName(curFormula);
                                        string formulaPath = formulaFolder + fileName3 + ".asset";
                                        try 
                                        { 
                                            AssetDatabase.CreateAsset(curFormula, formulaPath);
                                            curSkill.formulaStatModifiers[curStat][m] = curFormula;
                                        }
                                        catch
                                        {
                                            curSkill.formulaStatModifiers[curStat][m] = AssetDatabase.LoadMainAssetAtPath(formulaPath) as RPGFormula;
                                        }
                                    }
                                }
                            }
                        }
                        if (curSkill.newBasicAttackFormula != null)
                        {
                            for (int k = 0; k < curSkill.newBasicAttackFormula.Count; k++)
                            {
                                RPGFormula curFormula = curSkill.newBasicAttackFormula[k];
                                string fileName3 = RPGFormula.RPGFormulaCustomEditor.GetRPGFormulaFileName(curFormula);
                                string formulaPath = formulaFolder + fileName3 + ".asset";


                                try
                                {
                                    AssetDatabase.CreateAsset(curFormula, formulaPath);
                                    curSkill.newBasicAttackFormula[k] = curFormula;
                                }
                                catch
                                {
                                    curSkill.newBasicAttackFormula[k] = AssetDatabase.LoadMainAssetAtPath(formulaPath) as RPGFormula;
                                }
                            }
                        }
                        #endregion

                        AssetDatabase.CreateAsset(curSkill, subpath);

                        currentClass.skillList[j] = curSkill;
                    }
                    GameAssetsManager.instance.classes[i] = currentClass;
                    AssetDatabase.CreateAsset(currentClass, classpath);
                }
            }
        }

    }

#endif
    #endregion
}