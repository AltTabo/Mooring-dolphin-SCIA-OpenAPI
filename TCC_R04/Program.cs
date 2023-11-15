using Results64Enums;
using SCIA.OpenAPI;
using SCIA.OpenAPI.OpenAPIEnums;
using SCIA.OpenAPI.Results;
using SCIA.OpenAPI.StructureModelDefinition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using static TCC_R04.Janelas2;
using static TCC_R04.Valores;
using Environment = SCIA.OpenAPI.Environment;

namespace TCC_R04 //SciaEngineer.OpenApi.TCC
{
    internal class Program
    {
        private static string SenInstallationPath = "C:\\Program Files\\SCIA\\Engineer22.1\\";
        private static string SenTempFolder = "C:\\Users\\Gustavo\\ESA22.1\\Temp\\ADMsync\\";
        private static string SenEmptyProject_ORG = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\EsaProjects\\EmptyProject22.1.esad";
        public static DateTime now;
        public static string formattedDate;
        public static string SenEmptyProject;

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        public const int KEYEVENTF_KEYUP = 0x0002;
        public const byte VK_R = 0x52;
        public const byte VK_A = 0x41;
        public const byte VK_ALT = 0x12;
        public const byte VK_L = 0x4C;
        public const byte VK_C = 0x43;
        public const byte VK_TAB = 0x09;
        public const byte VK_DOWN = 0x28;
        public const byte VK_ENTER = 0x0D;

        static void Main(string[] args)
        {
            //now = DateTime.Now;
            //formattedDate = now.ToString("yyyyMMdd_HHmm");
            //SenEmptyProject = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\EsaProjects\\EmptyProject22.1_" + formattedDate + ".esad";
            //File.Copy(SenEmptyProject_ORG, SenEmptyProject, true);
            //string processToMonitor = "SCIA Engineer 22.1.2011";
            //Thread monitorThread = new Thread(() => MonitorProcess(processToMonitor));
            //monitorThread.Start();

            var isInitialized = Tools.InitializeApplication(SenInstallationPath);

            if (!isInitialized)
            {
                return;
            }

            Variaveis variaveis = new Variaveis(6, 5, 1.5, 75, 45, 7.7, 21.3, 20, 9.7, 1.5, 1, 4.57e6, 75.405e6, 1.5, 2.5, 5e3, 320, 320, 10e3);
            Variaveis_iter var_iter = new Variaveis_iter(10, 15, 0, 0, 2, 2, 1, 1);
            CreateCustomModel(variaveis, var_iter);

            Console.WriteLine();
            Console.WriteLine("Finished.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        //public static void CreateModelUsingOpenApi(EsaProject proj, Variaveis var)
        public static void CreateModelUsingOpenApi(Variaveis var)
        {
            now = DateTime.Now;
            formattedDate = now.ToString("yyyyMMdd_HHmm");
            SenEmptyProject = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\EsaProjects\\EmptyProject22.1_" + formattedDate + ".esad";
            File.Copy(SenEmptyProject_ORG, SenEmptyProject, true);
            string processToMonitor = "SCIA Engineer 22.1.2011";
            Thread monitorThread = new Thread(() => MonitorProcess(processToMonitor));
            monitorThread.Start();

            
            (Environment Environment, EsaProject Project) senData = Tools.StartSciaEngineer(SenInstallationPath, SenTempFolder, SenEmptyProject);


            Structure model = senData.Project.Model;
            #region CRIACAO DE MATERIAL, NOS E LAJE
            Material concreto = new Material(Guid.NewGuid(), "C40", 0, "C40");
            Material aco = new Material(Guid.NewGuid(), "A572 grade 50", 1, "A572 grade 50");

            foreach (var x in new List<Material> { concreto, aco }) { model.CreateMaterial(x); }

            StructNode node1 = new StructNode(Guid.NewGuid(), "Node1", 0, 0, 0);
            StructNode node2 = new StructNode(Guid.NewGuid(), "Node2", var.A, 0, 0);
            StructNode node3 = new StructNode(Guid.NewGuid(), "Node3", var.A, var.B, 0);
            StructNode node4 = new StructNode(Guid.NewGuid(), "Node4", 0, var.B, 0);

            foreach (var x in new List<StructNode> { node1, node2, node3, node4 }) { model.CreateNode(x); }

            var slab = new Slab(Guid.NewGuid(), "Laje", 0, concreto.Id, var.C, new Guid[4] { node1.Id, node2.Id, node3.Id, node4.Id });

            bool result = model.CreateSlab(slab);
            #endregion

            #region ESTACAS
            //A seguir serão definidos os nós dos pontos onde as estacas encostam a laje(plano XY).
            StructNode nodeE1L = new StructNode(Guid.NewGuid(), "NodeE1L", var.No_estaca_x, var.No_estaca_y, 0);
            StructNode nodeE2L = new StructNode(Guid.NewGuid(), "NodeE2L", var.A - var.No_estaca_x, var.No_estaca_y, 0);
            StructNode nodeE3L = new StructNode(Guid.NewGuid(), "NodeE3L", var.A - var.No_estaca_x, var.B - var.No_estaca_y, 0);
            StructNode nodeE4L = new StructNode(Guid.NewGuid(), "NodeE4L", var.No_estaca_x, var.B - var.No_estaca_y, 0);

            foreach (var x in new List<StructNode> { nodeE1L, nodeE2L, nodeE3L, nodeE4L }) { model.CreateNode(x); }

            /*
             * Estaca_Z
             */
            double estaca_Z = var.Estaca_comp() * Math.Sin((Math.PI * var.Estaca_alpha) / 180);
            double estaca_XY = var.Estaca_comp() * Math.Cos((Math.PI * var.Estaca_alpha) / 180);
            double estaca_X = estaca_XY * Math.Sin((Math.PI * var.Estaca_beta) / 180);
            double estaca_Y = estaca_XY * Math.Cos((Math.PI * var.Estaca_beta) / 180);

            Console.WriteLine(var.Estaca_comp() + " " + var.H_total() + " " + var.H_laje_solo + " " + var.H_solo + " " + var.H_agua + " " + var.H_conc );

            StructNode nodeE1 = new StructNode(Guid.NewGuid(), "NodeE1", var.No_estaca_x - estaca_X, var.No_estaca_y - estaca_Y, -estaca_Z);
            StructNode nodeE2 = new StructNode(Guid.NewGuid(), "NodeE2", var.A - var.No_estaca_x + estaca_X, var.No_estaca_y - estaca_Y, -estaca_Z);
            StructNode nodeE3 = new StructNode(Guid.NewGuid(), "NodeE3", var.A - var.No_estaca_x + estaca_X, var.B - var.No_estaca_y + estaca_Y, -estaca_Z);
            StructNode nodeE4 = new StructNode(Guid.NewGuid(), "NodeE4", var.No_estaca_x - estaca_X, var.B - var.No_estaca_y + estaca_Y, -estaca_Z);

            foreach (var x in new List<StructNode> { nodeE1, nodeE2, nodeE3, nodeE4 }) { model.CreateNode(x); }

            double estaca_conc_Z = var.Estaca_comp_conc() * Math.Sin((Math.PI * var.Estaca_alpha) / 180);
            double estaca_conc_XY = var.Estaca_comp_conc() * Math.Cos((Math.PI * var.Estaca_alpha) / 180);
            double estaca_conc_X = estaca_conc_XY * Math.Sin((Math.PI * var.Estaca_beta) / 180);
            double estaca_conc_Y = estaca_conc_XY * Math.Cos((Math.PI * var.Estaca_beta) / 180);

            StructNode nodeE1C = new StructNode(Guid.NewGuid(), "NodeE1C", var.No_estaca_x - estaca_conc_X, var.No_estaca_y - estaca_conc_Y, -estaca_conc_Z);
            StructNode nodeE2C = new StructNode(Guid.NewGuid(), "NodeE2C", var.A - var.No_estaca_x + estaca_conc_X, var.No_estaca_y - estaca_conc_Y, -estaca_conc_Z);
            StructNode nodeE3C = new StructNode(Guid.NewGuid(), "NodeE3C", var.A - var.No_estaca_x + estaca_conc_X, var.B - var.No_estaca_y + estaca_conc_Y, -estaca_conc_Z);
            StructNode nodeE4C = new StructNode(Guid.NewGuid(), "NodeE4C", var.No_estaca_x - estaca_conc_X, var.B - var.No_estaca_y + estaca_conc_Y, -estaca_conc_Z);

            foreach (var x in new List<StructNode> { nodeE1C, nodeE2C, nodeE3C, nodeE4C }) { model.CreateNode(x); }

            //formCode: CircularHollowSection = 3,
            //DescriptionID: CelsiusLargeCircularHollowSection = 103,

            var cs_O = new CrossSectionManufactured(Guid.NewGuid(), "CS_O", aco.Id, "LCHS(Ce)914/10.3", 3, 103);

            model.CreateCrossSection(cs_O);

            //ProfileType: Circle = 0,
            //Detailed: 888

            double[] diametro = { 0.888 };
            var cs_C = new CrossSectionParametric(Guid.NewGuid(), "CS_C", concreto.Id, 0, diametro);

            result = model.CreateCrossSection(cs_C);

            Beam estaca1C = new Beam(Guid.NewGuid(), "Estaca1C", cs_C.Id, new Guid[2] { nodeE1L.Id, nodeE1C.Id });
            Beam estaca1O = new Beam(Guid.NewGuid(), "Estaca1O", cs_O.Id, new Guid[2] { nodeE1C.Id, nodeE1.Id });
            Beam estaca2C = new Beam(Guid.NewGuid(), "Estaca2C", cs_C.Id, new Guid[2] { nodeE2L.Id, nodeE2C.Id });
            Beam estaca2O = new Beam(Guid.NewGuid(), "Estaca2O", cs_O.Id, new Guid[2] { nodeE2C.Id, nodeE2.Id });
            Beam estaca3C = new Beam(Guid.NewGuid(), "Estaca3C", cs_C.Id, new Guid[2] { nodeE3L.Id, nodeE3C.Id });
            Beam estaca3O = new Beam(Guid.NewGuid(), "Estaca3O", cs_O.Id, new Guid[2] { nodeE3C.Id, nodeE3.Id });
            Beam estaca4C = new Beam(Guid.NewGuid(), "Estaca4C", cs_C.Id, new Guid[2] { nodeE4L.Id, nodeE4C.Id });
            Beam estaca4O = new Beam(Guid.NewGuid(), "Estaca4O", cs_O.Id, new Guid[2] { nodeE4C.Id, nodeE4.Id });

            foreach (var x in new List<Beam> { estaca1C, estaca1O, estaca2C, estaca2O, estaca3C, estaca3O, estaca4C, estaca4O }) { model.CreateBeam(x); }
            #endregion

            #region CABECOS DE AMARRACAO
            StructNode nodeCA1L = new StructNode(Guid.NewGuid(), "NodeCA1L", var.No_cabeco_x, var.No_cabeco_y, 0);
            StructNode nodeCA2L = new StructNode(Guid.NewGuid(), "NodeCA2L", var.A - var.No_cabeco_x, var.No_cabeco_y, 0);
            StructNode nodeCA1 = new StructNode(Guid.NewGuid(), "NodeCA1", var.No_cabeco_x, var.No_cabeco_y, var.H_cabeco());
            StructNode nodeCA2 = new StructNode(Guid.NewGuid(), "NodeCA2", var.A - var.No_cabeco_x, var.No_cabeco_y, var.H_cabeco());

            foreach (var x in new List<StructNode> { nodeCA1L, nodeCA2L, nodeCA1, nodeCA2 }) { model.CreateNode(x); }

            Beam cabeco1 = new Beam(Guid.NewGuid(), "Cabeco1", cs_C.Id, new Guid[2] { nodeCA1L.Id, nodeCA1.Id });
            Beam cabeco2 = new Beam(Guid.NewGuid(), "Cabeco2", cs_C.Id, new Guid[2] { nodeCA2L.Id, nodeCA2.Id });

            foreach (var x in new List<Beam> { cabeco1, cabeco2 }) { model.CreateBeam(x); }
            #endregion

            #region CONEXOES 

            //StructuralPointConnection nolaje1 = new StructuralPointConnection(Guid.NewGuid(), "no_conexao1", UnitsNet.Length.FromMeters(no_estaca_x), UnitsNet.Length.FromMeters(no_estaca_y), UnitsNet.Length.Zero);
            //StructuralPointConnection nolaje2 = new StructuralPointConnection(Guid.NewGuid(), "no_conexao2", UnitsNet.Length.FromMeters(no_estaca_x), UnitsNet.Length.FromMeters(no_estaca_y), UnitsNet.Length.Zero);



            //RelConnectsRigidLink intersecao = new RelConnectsRigidLink(Guid.NewGuid(), "RL1", new[] { nolaje1 , nolaje2 });



            #endregion

            #region APOIOS
            /*
            Apoio:
            Quantidade: 9
            Distância entre apoios: Comprimento enterrado / 8
            Rigidez inicial
            Rigidez final
            Variação linear da rigidez (Ri = Ri-1 + (Rn-Ri)/8)
            Rx = Ry = Rz = Livre para todos
            X = Y = Flexível para todos (Rigidez = Ri)
            Z = Livre para todos exceto x = 0,00m
             */

            double estaca_comp_O = var.Estaca_comp() - var.Estaca_comp_conc(); //É o comprimento total da estaca oca
            double estaca_comp_solo = var.H_solo / Math.Sin(Math.PI * var.Estaca_alpha / 180); //É o comprimento da estaca oca enterrada
            double estaca_percentual = estaca_comp_solo / estaca_comp_O;
            double epi = estaca_percentual / 8; //epi = intervalo percentual entre apoios

            double rigidez_diff = var.Rigidez_f - var.Rigidez_i;
            double rigidez_int = rigidez_diff / 8;

            //Console.WriteLine("estaca_comp_O");
            //Console.WriteLine(estaca_comp_O);
            //Console.WriteLine("estaca_comp_solo");
            //Console.WriteLine(estaca_comp_solo);
            //Console.WriteLine("estaca_percentual");
            //Console.WriteLine(estaca_percentual);
            //Console.WriteLine("epi");
            //Console.WriteLine(epi);

            //Console.WriteLine("epi");

            LineSupport[,] ApoioE = new LineSupport[4, 9];
            PointSupport[] ApoioFinal = new PointSupport[4];
            string nomeApoio;
            double pontoinicial;
            double pontofinal;
            double rigidez;
            eConstraintType constraintz;

            ApiGuid[] estaca_apoio = new ApiGuid[4];
            estaca_apoio[0] = estaca1O.Id;
            estaca_apoio[1] = estaca2O.Id;
            estaca_apoio[2] = estaca3O.Id;
            estaca_apoio[3] = estaca4O.Id;

            ApiGuid[] no_apoio = new ApiGuid[4];
            no_apoio[0] = nodeE1.Id; no_apoio[1] = nodeE2.Id;
            no_apoio[2] = nodeE3.Id; no_apoio[3] = nodeE4.Id;

            for (int i = 3; i >= 0; i--)
            {
                for (int j = 8; j >= 0; j--)
                {
                    nomeApoio = "ApoioE" + i + "_" + j;
                    pontofinal = 1 - (j * epi) + epi;
                    pontoinicial = pontofinal - epi + 0.00001;
                    rigidez = (var.Rigidez_f - j * rigidez_int) / (estaca_comp_solo / 8);
                    if (j == 0)
                    {
                        constraintz = eConstraintType.Rigid;
                        ApoioFinal[i] = new PointSupport(Guid.NewGuid(), nomeApoio, no_apoio[i]) { ConstraintX = eConstraintType.Flexible, StiffnessX = var.Rigidez_f, ConstraintY = eConstraintType.Flexible, StiffnessY = var.Rigidez_f, ConstraintZ = constraintz, ConstraintRx = eConstraintType.Free, ConstraintRy = eConstraintType.Free, ConstraintRz = eConstraintType.Free };
                        model.CreatePointSupport(ApoioFinal[i]);
                    }
                    else
                    {
                        constraintz = eConstraintType.Free;
                        ApoioE[i, j] = new LineSupport(Guid.NewGuid(), nomeApoio, estaca_apoio[i]) { CoordinateDefinition = eCoordinateDefinition.Relative, StartPoint = pontoinicial, EndPoint = pontofinal, Origin = eLineOrigin.FromStart, ConstraintX = eConstraintType.Flexible, StiffnessX = rigidez, ConstraintY = eConstraintType.Flexible, StiffnessY = rigidez, ConstraintZ = constraintz, ConstraintRx = eConstraintType.Free, ConstraintRy = eConstraintType.Free, ConstraintRz = eConstraintType.Free };
                        model.CreateLineSupport(ApoioE[i, j]);
                    }
                }
            }
            #endregion

            #region GRUPOS DE CARGA
            LoadGroup LG1 = new LoadGroup(Guid.NewGuid(), "LG1_PERMANENTE", (int)eLoadGroup_Load.eLoadGroup_Load_Permanent);
            LoadGroup LG2 = new LoadGroup(Guid.NewGuid(), "LG2_CABECO_1", (int)eLoadGroup_Load.eLoadGroup_Load_Variable);
            LoadGroup LG3 = new LoadGroup(Guid.NewGuid(), "LG3_CABECO_2", (int)eLoadGroup_Load.eLoadGroup_Load_Variable);
            LoadGroup LG4 = new LoadGroup(Guid.NewGuid(), "LG4_CORRENTE", (int)eLoadGroup_Load.eLoadGroup_Load_Variable);
            LoadGroup LG5 = new LoadGroup(Guid.NewGuid(), "LG5_TEMPERATURA", (int)eLoadGroup_Load.eLoadGroup_Load_Variable);
            LoadGroup LG6 = new LoadGroup(Guid.NewGuid(), "LG6_SOBRECARGA", (int)eLoadGroup_Load.eLoadGroup_Load_Variable);

            foreach (var x in new List<LoadGroup> { LG1, LG2, LG3, LG4, LG5, LG6 }) { model.CreateLoadGroup(x); }
            #endregion

            #region CASOS DE CARGA
            //Type of action caused by load case:
            //    Permanent = 0,
            //    Variable = 1,
            //    Accidental = 2
            //Type of load case:
            //    SelfWeight = 0,
            //    Standard = 1,
            //    Prestress = 2,
            //    Dynamic = 3,
            //    PrimaryEffect = 4,
            //    Static = 5

            LoadCase PP = new LoadCase(Guid.NewGuid(), "PP_PesoProprio", (int)eLoadCase_ActionType.eLoadCase_ActionType_Permanent, LG1.Id, 0);
            LoadCase SC = new LoadCase(Guid.NewGuid(), "SC_SobreCarga", (int)eLoadCase_ActionType.eLoadCase_ActionType_Variable, LG6.Id, 5);
            LoadCase RET = new LoadCase(Guid.NewGuid(), "RET_Retracao", (int)eLoadCase_ActionType.eLoadCase_ActionType_Permanent, LG1.Id, 1);
            LoadCase TEMP_MAIS = new LoadCase(Guid.NewGuid(), "TEMP_MAIS_Temperatura", (int)eLoadCase_ActionType.eLoadCase_ActionType_Variable, LG5.Id, 5);
            LoadCase TEMP_MENOS = new LoadCase(Guid.NewGuid(), "TEMP_MENOS_Temperatura", (int)eLoadCase_ActionType.eLoadCase_ActionType_Variable, LG5.Id, 5);
            LoadCase COR_LONG_MAIS = new LoadCase(Guid.NewGuid(), "COR_LONG_MAIS_Corrente", (int)eLoadCase_ActionType.eLoadCase_ActionType_Variable, LG4.Id, 5) { Duration = eDuration.Short };
            LoadCase COR_LONG_MENOS = new LoadCase(Guid.NewGuid(), "COR_LONG_MENOS_Corrente", (int)eLoadCase_ActionType.eLoadCase_ActionType_Variable, LG4.Id, 5) { Duration = eDuration.Short };
            LoadCase COR_TRANSV_MAIS = new LoadCase(Guid.NewGuid(), "COR_TRANSV_MAIS_Corrente", (int)eLoadCase_ActionType.eLoadCase_ActionType_Variable, LG4.Id, 5) { Duration = eDuration.Short };
            LoadCase COR_TRANSV_MENOS = new LoadCase(Guid.NewGuid(), "COR_TRANSV_MENOS_Corrente", (int)eLoadCase_ActionType.eLoadCase_ActionType_Variable, LG4.Id, 5) { Duration = eDuration.Short };
            foreach (var x in new List<LoadCase> { PP, SC, RET, TEMP_MAIS, TEMP_MENOS, COR_LONG_MAIS, COR_LONG_MENOS, COR_TRANSV_MAIS, COR_TRANSV_MENOS }) { model.CreateLoadCase(x); }

            string nomeCAB;
            LoadCase[,] CAB = new LoadCase[2, 10];

            for (int i = 1; i <= 2; i++)
            {
                for (int j = 0; j <= 9; j++)
                {
                    if (j == 4)
                    {
                        nomeCAB = "CAB" + i + "_" + j + "5";
                    }
                    else
                    {
                        nomeCAB = "CAB" + i + "_" + j + "0";
                    }
                    switch (i)
                    {
                        case 1:
                            CAB[i - 1, j] = new LoadCase(Guid.NewGuid(), nomeCAB, (int)eLoadCase_ActionType.eLoadCase_ActionType_Variable, LG2.Id, 5) { Duration = eDuration.Short };
                            break;
                        case 2:
                            CAB[i - 1, j] = new LoadCase(Guid.NewGuid(), nomeCAB, (int)eLoadCase_ActionType.eLoadCase_ActionType_Variable, LG1.Id, 5) { Duration = eDuration.Short };
                            break;
                    }

                    model.CreateLoadCase(CAB[i - 1, j]);

                }
            }
            #endregion

            #region COMBINACOES DE CARGA

            #region ESTADO LIMITE ULTIMO
            CombinationItem[] combinationItems_ELU1 = new CombinationItem[] { };
            combinationItems_ELU1 = combinationItems_ELU1.Append(new CombinationItem(PP.Id, 1.4)).ToArray();
            combinationItems_ELU1 = combinationItems_ELU1.Append(new CombinationItem(RET.Id, 1.4)).ToArray();

            Combination ELU_1 = new Combination(Guid.NewGuid(), "ELU_1", combinationItems_ELU1) { Category = eLoadCaseCombinationCategory.AccordingNationalStandard, NationalStandard = eLoadCaseCombinationStandard.IbcLrfdUltimate };
            model.CreateCombination(ELU_1);

            CombinationItem[] combinationItems_ELU2 = new CombinationItem[] { };
            combinationItems_ELU2 = combinationItems_ELU2.Append(new CombinationItem(PP.Id, 1.0)).ToArray();
            combinationItems_ELU2 = combinationItems_ELU2.Append(new CombinationItem(RET.Id, 1.0)).ToArray();

            Combination ELU_2 = new Combination(Guid.NewGuid(), "ELU_2", combinationItems_ELU2) { Category = eLoadCaseCombinationCategory.AccordingNationalStandard, NationalStandard = eLoadCaseCombinationStandard.IbcLrfdUltimate };
            model.CreateCombination(ELU_2);


            CombinationItem[] combinationItems_ELU = new CombinationItem[29];
            List<CombinationItem> Lista_Combinacoes = new List<CombinationItem> { };
            Combination[] ELU = new Combination[8];
            int k = 0;
            int p;
            double coef_perm;
            double coef_temp = 0.72;
            double coef_cabeco1;
            double coef_cabeco2;
            double coef_corrente;
            double coef_sobrecarga;

            for (int n = 0; n <= 1; n++)
            {
                for (int m = 0; m <= 3; m++)
                {

                    if (n == 0) { coef_perm = 1.4; } else { coef_perm = 1.0; }

                    coef_cabeco1 = 1.05; coef_cabeco2 = 1.05; coef_corrente = 1.05; coef_sobrecarga = 1.05;

                    switch (m)
                    {
                        case 0:
                            coef_cabeco1 = 1.5;
                            break;
                        case 1:
                            coef_cabeco2 = 1.5;
                            break;
                        case 2:
                            coef_corrente = 1.5;
                            break;
                        case 3:
                            coef_sobrecarga = 1.5;
                            break;
                    }

                    combinationItems_ELU[0] = new CombinationItem(PP.Id, coef_perm);
                    combinationItems_ELU[1] = new CombinationItem(RET.Id, coef_perm);
                    combinationItems_ELU[2] = new CombinationItem(TEMP_MAIS.Id, coef_temp);
                    combinationItems_ELU[3] = new CombinationItem(TEMP_MENOS.Id, coef_temp);
                    combinationItems_ELU[4] = new CombinationItem(SC.Id, coef_sobrecarga);
                    combinationItems_ELU[5] = new CombinationItem(COR_LONG_MAIS.Id, coef_corrente);
                    combinationItems_ELU[6] = new CombinationItem(COR_LONG_MENOS.Id, coef_corrente);
                    combinationItems_ELU[7] = new CombinationItem(COR_TRANSV_MAIS.Id, coef_corrente);
                    combinationItems_ELU[8] = new CombinationItem(COR_TRANSV_MENOS.Id, coef_corrente);
                    for (int j = 0; j <= 9; j++)
                    {
                        combinationItems_ELU[2 * j + 9] = new CombinationItem(CAB[0, j].Id, coef_cabeco1);
                        combinationItems_ELU[2 * j + 10] = new CombinationItem(CAB[1, j].Id, coef_cabeco2);

                    }

                    p = k + 3;

                    for (int q = 0; q <= 28; q++)
                    {
                        Lista_Combinacoes.Add(combinationItems_ELU[q]);
                    }
                    ELU[k] = new Combination(Guid.NewGuid(), "ELU_" + p, Lista_Combinacoes) { Category = eLoadCaseCombinationCategory.AccordingNationalStandard, NationalStandard = eLoadCaseCombinationStandard.IbcLrfdUltimate };
                    model.CreateCombination(ELU[k]);
                    k++;
                    Lista_Combinacoes.Clear();
                }
            }
            #endregion



            #region ESTADO LIMITE DE SERVIÇO - RARAS
            CombinationItem[] combinationItems_ELSR1 = new CombinationItem[] { };
            combinationItems_ELSR1 = combinationItems_ELSR1.Append(new CombinationItem(PP.Id, 1.0)).ToArray();
            combinationItems_ELSR1 = combinationItems_ELSR1.Append(new CombinationItem(RET.Id, 1.0)).ToArray();

            Combination ELSR_1 = new Combination(Guid.NewGuid(), "ELSR_1", combinationItems_ELSR1) { Category = eLoadCaseCombinationCategory.AccordingNationalStandard, NationalStandard = eLoadCaseCombinationStandard.EnSlsCharacteristic };
            model.CreateCombination(ELSR_1);

            CombinationItem[] combinationItems_ELSR = new CombinationItem[29];
            Combination[] ELSR = new Combination[5];

            k = 0;
            coef_perm = 1;

            for (int m = 0; m <= 4; m++)
            {

                coef_cabeco1 = 0.6; coef_cabeco2 = 0.6; coef_corrente = 0.6; coef_sobrecarga = 0.6; coef_temp = 0.5;

                switch (m)
                {
                    case 0:
                        coef_cabeco1 = 1;
                        break;
                    case 1:
                        coef_cabeco2 = 1;
                        break;
                    case 2:
                        coef_corrente = 1;
                        break;
                    case 3:
                        coef_sobrecarga = 1;
                        break;
                    case 4:
                        coef_temp = 1;
                        break;
                }

                combinationItems_ELSR[0] = new CombinationItem(PP.Id, coef_perm);
                combinationItems_ELSR[1] = new CombinationItem(RET.Id, coef_perm);
                combinationItems_ELSR[2] = new CombinationItem(TEMP_MAIS.Id, coef_temp);
                combinationItems_ELSR[3] = new CombinationItem(TEMP_MENOS.Id, coef_temp);
                combinationItems_ELSR[4] = new CombinationItem(SC.Id, coef_sobrecarga);
                combinationItems_ELSR[5] = new CombinationItem(COR_LONG_MAIS.Id, coef_corrente);
                combinationItems_ELSR[6] = new CombinationItem(COR_LONG_MENOS.Id, coef_corrente);
                combinationItems_ELSR[7] = new CombinationItem(COR_TRANSV_MAIS.Id, coef_corrente);
                combinationItems_ELSR[8] = new CombinationItem(COR_TRANSV_MENOS.Id, coef_corrente);
                for (int j = 0; j <= 9; j++)
                {
                    combinationItems_ELSR[2 * j + 9] = new CombinationItem(CAB[0, j].Id, coef_cabeco1);
                    combinationItems_ELSR[2 * j + 10] = new CombinationItem(CAB[1, j].Id, coef_cabeco2);

                }

                p = k + 2;

                for (int q = 0; q <= 28; q++)
                {
                    Lista_Combinacoes.Add(combinationItems_ELSR[q]);
                }
                ELSR[k] = new Combination(Guid.NewGuid(), "ELSR_" + p, Lista_Combinacoes) { Category = eLoadCaseCombinationCategory.AccordingNationalStandard, NationalStandard = eLoadCaseCombinationStandard.IbcAsdServiceability };
                model.CreateCombination(ELSR[k]);
                k++;
                Lista_Combinacoes.Clear();
            }
            #endregion

            #region ESTADO LIMITE DE SERVIÇO - QUASE PERMANENTE
            CombinationItem[] combinationItems_ELSQ1 = new CombinationItem[] { };
            combinationItems_ELSQ1 = combinationItems_ELSQ1.Append(new CombinationItem(PP.Id, 1.0)).ToArray();
            combinationItems_ELSQ1 = combinationItems_ELSQ1.Append(new CombinationItem(RET.Id, 1.0)).ToArray();

            Combination ELSQ_1 = new Combination(Guid.NewGuid(), "ELSQ_1", combinationItems_ELSQ1) { Category = eLoadCaseCombinationCategory.AccordingNationalStandard, NationalStandard = eLoadCaseCombinationStandard.EnUlsSetB };
            model.CreateCombination(ELSQ_1);

            CombinationItem[] combinationItems_ELSQ = new CombinationItem[29];
            Combination[] ELSQ = new Combination[1];

            k = 0;
            coef_perm = 1;
            coef_sobrecarga = coef_corrente = coef_cabeco1 = coef_cabeco2 = 0.4;
            coef_temp = 0.3;

            combinationItems_ELSQ[0] = new CombinationItem(PP.Id, coef_perm);
            combinationItems_ELSQ[1] = new CombinationItem(RET.Id, coef_perm);
            combinationItems_ELSQ[2] = new CombinationItem(TEMP_MAIS.Id, coef_temp);
            combinationItems_ELSQ[3] = new CombinationItem(TEMP_MENOS.Id, coef_temp);
            combinationItems_ELSQ[4] = new CombinationItem(SC.Id, coef_sobrecarga);
            combinationItems_ELSQ[5] = new CombinationItem(COR_LONG_MAIS.Id, coef_corrente);
            combinationItems_ELSQ[6] = new CombinationItem(COR_LONG_MENOS.Id, coef_corrente);
            combinationItems_ELSQ[7] = new CombinationItem(COR_TRANSV_MAIS.Id, coef_corrente);
            combinationItems_ELSQ[8] = new CombinationItem(COR_TRANSV_MENOS.Id, coef_corrente);
            for (int j = 0; j <= 9; j++)
            {
                combinationItems_ELSQ[2 * j + 9] = new CombinationItem(CAB[0, j].Id, coef_cabeco1);
                combinationItems_ELSQ[2 * j + 10] = new CombinationItem(CAB[1, j].Id, coef_cabeco2);

            }

            for (int q = 0; q <= 28; q++)
            {
                Lista_Combinacoes.Add(combinationItems_ELSQ[q]);
            }

            p = k + 2;

            ELSQ[k] = new Combination(Guid.NewGuid(), "ELSQ_" + p, Lista_Combinacoes) { Category = eLoadCaseCombinationCategory.AccordingNationalStandard, NationalStandard = eLoadCaseCombinationStandard.IbcAsdServiceability };
            model.CreateCombination(ELSQ[k]);

            Lista_Combinacoes.Clear();
            #endregion

            #region ESTADO LIMITE DE SERVIÇO - FREQUENTE
            CombinationItem[] combinationItems_ELSF1 = new CombinationItem[] { };
            combinationItems_ELSF1 = combinationItems_ELSF1.Append(new CombinationItem(PP.Id, 1.0)).ToArray();
            combinationItems_ELSF1 = combinationItems_ELSF1.Append(new CombinationItem(RET.Id, 1.0)).ToArray();

            Combination ELSF_1 = new Combination(Guid.NewGuid(), "ELSF_1", combinationItems_ELSF1) { Category = eLoadCaseCombinationCategory.AccordingNationalStandard, NationalStandard = eLoadCaseCombinationStandard.EnSlsFrequent};
            model.CreateCombination(ELSF_1);

            CombinationItem[] combinationItems_ELSF = new CombinationItem[29];
            Combination[] ELSF = new Combination[5];

            k = 0;
            coef_perm = 1;

            for (int m = 0; m <= 4; m++)
            {

                coef_cabeco1 = 0.4; coef_cabeco2 = 0.4; coef_corrente = 0.4; coef_sobrecarga = 0.4; coef_temp = 0.3;

                switch (m)
                {
                    case 0:
                        coef_cabeco1 = 0.6;
                        break;
                    case 1:
                        coef_cabeco2 = 0.6;
                        break;
                    case 2:
                        coef_corrente = 0.6;
                        break;
                    case 3:
                        coef_sobrecarga = 0.6;
                        break;
                    case 4:
                        coef_temp = 0.5;
                        break;
                }

                combinationItems_ELSF[0] = new CombinationItem(PP.Id, coef_perm);
                combinationItems_ELSF[1] = new CombinationItem(RET.Id, coef_perm);
                combinationItems_ELSF[2] = new CombinationItem(TEMP_MAIS.Id, coef_temp);
                combinationItems_ELSF[3] = new CombinationItem(TEMP_MENOS.Id, coef_temp);
                combinationItems_ELSF[4] = new CombinationItem(SC.Id, coef_sobrecarga);
                combinationItems_ELSF[5] = new CombinationItem(COR_LONG_MAIS.Id, coef_corrente);
                combinationItems_ELSF[6] = new CombinationItem(COR_LONG_MENOS.Id, coef_corrente);
                combinationItems_ELSF[7] = new CombinationItem(COR_TRANSV_MAIS.Id, coef_corrente);
                combinationItems_ELSF[8] = new CombinationItem(COR_TRANSV_MENOS.Id, coef_corrente);
                for (int j = 0; j <= 9; j++)
                {
                    combinationItems_ELSF[2 * j + 9] = new CombinationItem(CAB[0, j].Id, coef_cabeco1);
                    combinationItems_ELSF[2 * j + 10] = new CombinationItem(CAB[1, j].Id, coef_cabeco2);

                }

                p = k + 2;

                for (int q = 0; q <= 28; q++)
                {
                    Lista_Combinacoes.Add(combinationItems_ELSF[q]);
                }
                ELSF[k] = new Combination(Guid.NewGuid(), "ELSF_" + p, Lista_Combinacoes) { Category = eLoadCaseCombinationCategory.AccordingNationalStandard, NationalStandard = eLoadCaseCombinationStandard.IbcAsdServiceability };
                model.CreateCombination(ELSF[k]);
                k++;
                Lista_Combinacoes.Clear();
            }
            #endregion

            #endregion

            #region CARGAS

            #region SOBRECARGA
            //Direction: X = 0, Y = 1, Z = 2
            SurfaceLoad sc = new SurfaceLoad(Guid.NewGuid(), "sobrecarga", -var.Sobrecarga, SC.Id, slab.Id, 2);
            model.CreateSurfaceLoad(sc);
            #endregion

            #region CORRENTE
            //double estaca_comp_O = estaca_comp - estaca_comp_conc;
            //double estaca_comp_solo = h_solo / Math.Sin(Math.PI * estaca_alpha / 180);
            double estaca_comp_O_agua = estaca_comp_O - estaca_comp_solo; //É o comprimento da estaca oca molhada
            double estaca_comp_agua = var.H_agua / Math.Sin(Math.PI * var.Estaca_alpha / 180); //É o comprimento total da estaca molhada (oca e concreto)
            double estaca_comp_conc_agua = estaca_comp_agua - estaca_comp_O_agua; //É o comprimento molhado da estaca concretada

            double[] estaca_relativo = new double[2];
            estaca_relativo[0] = estaca_comp_O_agua / estaca_comp_O; estaca_relativo[1] = estaca_comp_conc_agua / var.Estaca_comp_conc();

            ApiGuid[,] estaca = new ApiGuid[2, 4];
            estaca[0, 0] = estaca1O.Id; estaca[1, 0] = estaca1C.Id;
            estaca[0, 1] = estaca2O.Id; estaca[1, 1] = estaca2C.Id;
            estaca[0, 2] = estaca3O.Id; estaca[1, 2] = estaca3C.Id;
            estaca[0, 3] = estaca4O.Id; estaca[1, 3] = estaca4C.Id;

            ApiGuid[] corrente = new ApiGuid[4];
            corrente[0] = COR_LONG_MAIS.Id; corrente[1] = COR_LONG_MENOS.Id;
            corrente[2] = COR_TRANSV_MAIS.Id; corrente[3] = COR_TRANSV_MENOS.Id;

            double[] forca = new double[4];
            forca[0] = var.Corrente_long; forca[1] = -var.Corrente_long;
            forca[2] = var.Corrente_transv; forca[3] = -var.Corrente_transv;

            eDirection[] direcao = new eDirection[4];
            direcao[0] = eDirection.Y; direcao[1] = eDirection.Y;
            direcao[2] = eDirection.X; direcao[3] = eDirection.X;

            eLineOrigin[] origem = new eLineOrigin[2];
            origem[0] = eLineOrigin.FromStart; origem[1] = eLineOrigin.FromEnd;

            LineLoadOnBeam[,,] carga_corrente = new LineLoadOnBeam[2, 4, 4]; //2 Estacas (oca e concreto); 4 estacas; 4 direções de corrente)

            for (int i = 0; i <= 1; i++)
            {
                for (int j = 0; j <= 3; j++)
                {
                    for (k = 0; k <= 3; k++)
                    {
                        carga_corrente[i, j, k] = new LineLoadOnBeam(Guid.NewGuid(), "COR_" + i + "_" + j + "_" + k)
                        {
                            Member = estaca[i, j],
                            LoadCase = corrente[k],
                            Distribution = eLineLoadDistribution.Uniform,
                            Value1 = forca[k],
                            //Value2 = -12500,
                            CoordinateDefinition = eCoordinateDefinition.Relative,
                            StartPoint = 0,
                            EndPoint = estaca_relativo[i],
                            CoordinationSystem = eCoordinationSystem.GCS,
                            Direction = direcao[k],
                            Origin = origem[i],
                            Location = eLineLoadLocation.Length,
                            EccentricityEy = 0.0,
                            EccentricityEz = 0.0
                        };

                        model.CreateLineLoad(carga_corrente[i, j, k]);
                    }
                }
            }
            #endregion

            #region CABECOS DE AMARRACAO
            //PointLoadInNode Forca_teste = new PointLoadInNode(Guid.NewGuid(), "ForcaTeste", forca_cab, CAB[0, 0].Id, nodeCA1.Id, (int)eDirection.X);
            //model.CreatePointLoadInNode(Forca_teste);

            PointLoadInNode[,,] carga_cabeco = new PointLoadInNode[2, 10, 2]; //2 cabecos; 10 angulos de amarracao; 2 componentes (X e Y)
            double theta;

            ApiGuid[] cabeco = new ApiGuid[2];
            cabeco[0] = nodeCA1.Id; cabeco[1] = nodeCA2.Id;

            for (int i = 0; i <= 1; i++)
            {
                for (int j = 0; j <= 9; j++)
                {
                    if (j == 4)
                    {
                        theta = 45;
                    }
                    else
                    {
                        theta = j * 10;
                    }

                    double forca_x = var.Forca_cab * Math.Cos(Math.PI * theta / 180);
                    double forca_y = var.Forca_cab * Math.Sin(Math.PI * theta / 180);

                    if (i == 0) { forca_x = -forca_x; }

                    if (forca_x != 0)
                    {
                        carga_cabeco[i, j, 0] = new PointLoadInNode(Guid.NewGuid(), "FCABX_" + i + "_" + j, forca_x, CAB[i, j].Id, cabeco[i], (int)eDirection.X);
                        model.CreatePointLoadInNode(carga_cabeco[i, j, 0]);
                    }

                    if (forca_y != 0)
                    {
                        carga_cabeco[i, j, 1] = new PointLoadInNode(Guid.NewGuid(), "FCABY_" + i + "_" + j, forca_y, CAB[i, j].Id, cabeco[i], (int)eDirection.Y);
                        model.CreatePointLoadInNode(carga_cabeco[i, j, 1]);
                    }
                }
            }
            #endregion

            #region TEMPERATURA

            //StructuralSurfaceActionThermal carga_temp_mais = new StructuralSurfaceActionThermal(Guid.NewGuid(), "TEMPERATURA_MAIS", slab, TEMP_MAIS, UnitsNet.Temperature.FromDegreesCelsius(15));
            //StructuralSurfaceActionThermal carga_temp_menos = new StructuralSurfaceActionThermal(Guid.NewGuid(), "TEMPERATURA_MENOS", slab, TEMP_MENOS, UnitsNet.Temperature.FromDegreesCelsius(-15));

            #endregion

            #endregion

            #region CALCULO
            //MACRO - CALCULAR + CONECTAR MEMBROS (OpenAPI não permite conectar os membros)

            //Detecta a caixa de diálogo para prosseguir, em segundo plano
            /*string*/ processToMonitor = "SCIA Engineer 22.1.2011- student version (64 bit version)";
            /*Thread*/ monitorThread = new Thread(() => MonitorProcess(processToMonitor));
            monitorThread.Start();

            senData.Project.Model.RefreshModel_ToSCIAEngineer();
           
            int tempo = 300;
            Thread.Sleep(tempo);

            // Pressiona a tecla Alt
            keybd_event(VK_ALT, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo);

            // Pressiona a tecla R
            keybd_event(VK_R, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo);

            // Libera a tecla R
            keybd_event(VK_A, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(tempo);

            // Pressiona a tecla A
            keybd_event(VK_A, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo);

            // Libera a tecla A
            keybd_event(VK_A, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(tempo);

            // Pressiona a tecla C
            keybd_event(VK_C, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo);

            // Libera a tecla C
            keybd_event(VK_C, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(tempo);

            // Libera a tecla Alt
            keybd_event(VK_ALT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(tempo);

            // Pressiona a tecla TAB
            keybd_event(VK_TAB, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Libera a tecla TAB
            keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Pressiona a tecla TAB
            keybd_event(VK_TAB, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Libera a tecla TAB
            keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Pressiona a tecla DOWN
            keybd_event(VK_DOWN, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Libera a tecla DOWN
            keybd_event(VK_DOWN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Pressiona a tecla DOWN
            keybd_event(VK_DOWN, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Libera a tecla DOWN
            keybd_event(VK_DOWN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Pressiona a tecla DOWN
            keybd_event(VK_DOWN, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Libera a tecla DOWN
            keybd_event(VK_DOWN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Pressiona a tecla DOWN
            keybd_event(VK_DOWN, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Libera a tecla DOWN
            keybd_event(VK_DOWN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Pressiona a tecla ENTER
            keybd_event(VK_ENTER, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Libera a tecla ENTER
            keybd_event(VK_ENTER, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Pressiona a tecla TAB
            keybd_event(VK_TAB, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Libera a tecla TAB
            keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(tempo / 3);

            // Pressiona a tecla ENTER
            keybd_event(VK_ENTER, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo);

            // Libera a tecla ENTER
            keybd_event(VK_ENTER, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            //Janelas.Enumeracao();
            //Thread.Sleep(tempo * 1000);
            //Detecta a caixa de diálogo para prosseguir
            string Jan_EndofAnalysis = "SCIA Engineer:  End of analysis";
            bool teste = false;
            while (teste ==false)
            {
                Thread.Sleep(tempo);
                teste = Janelas2.Enumeracao(Jan_EndofAnalysis);

            }

            Thread.Sleep(tempo);

            // Pressiona a tecla ENTER
            keybd_event(VK_ENTER, 0, 0, UIntPtr.Zero);
            Thread.Sleep(tempo);

            // Libera a tecla ENTER
            keybd_event(VK_ENTER, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            #endregion

            #region RESULTADOS
            List<Beam> Lista_Estacas = new List<Beam> { estaca1C, estaca2C, estaca3C, estaca4C, estaca1O, estaca2O, estaca3O, estaca4O };

            //List<Beam> Lista_Estacas = new List<Beam> { estaca1C };

            List<Combination> Lista_ELU = new List<Combination> { ELU_1, ELU_2 };
            for (int j = 0; j <= 7; j++) { Lista_ELU.Add(ELU[j]); }

            //List<Combination> Lista_ELU = new List<Combination> { ELU[6] };

            List<Combination> Lista_ELSR = new List<Combination> { ELSR_1 };
            for (int j = 0; j <= 4; j++) { Lista_ELSR.Add(ELSR[j]); }

            List<Combination> Lista_ELSQ = new List<Combination> { ELSQ_1, ELSQ[0] };

            List<Combination> Lista_ELSF = new List<Combination> { ELSF_1 };
            for (int j = 0; j <= 4; j++) { Lista_ELSF.Add(ELSF[j]); }

            //List<List<Combination>> Envoltorias = new List<List<Combination>> { Lista_ELU, Lista_ELSR, Lista_ELSQ, Lista_ELSF };
            List<List<Combination>> Envoltorias = new List<List<Combination>> { Lista_ELU };

            senData.Project.Model.RefreshModel_FromSCIAEngineer();
            //proj.RunCalculation();

            string SenEmptyProject_dir = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\EsaProjects\\";
            string fileName = "log";
            string fileName_res = "log_res";
            string fileName_res_cons = "log_res_cons";
            //DateTime now = DateTime.Now;
            //string formattedDate = now.ToString("yyyyMMdd_HHmm");
            fileName = fileName + "_" + formattedDate + ".txt";
            string filePath = Path.Combine(SenEmptyProject_dir, fileName);
            fileName_res = fileName_res + "_" + formattedDate + ".txt";
            string filePath_res = Path.Combine(SenEmptyProject_dir, fileName_res);
            fileName_res_cons = fileName_res_cons + "_" + formattedDate + ".txt";
            string filePath_res_cons = Path.Combine(SenEmptyProject_dir, fileName_res_cons);

            string linha_JV = "##############################################################################################################";
            string linha;
            
            foreach (Beam x in Lista_Estacas)
            {
                linha = x.Name;
                File.AppendAllText(filePath, linha + System.Environment.NewLine);
                foreach (Combination y in Lista_ELU)
                {
                    linha = y.Name;
                    File.AppendAllText(filePath, linha + System.Environment.NewLine);
                }
            }
            

            //OpenApiE2EResults storage = new OpenApiE2EResults();

            using (ResultsAPI resultsApi = senData.Project.Model.InitializeResultsAPI())
            {
                #region VARIAÇÃO DE ENVOLTÓRIAS

                List<Resultados> Lista_resultados = new List<Resultados> { };

                foreach(List<Combination> Envoltoria in Envoltorias)
                {

                    #region ESTACAS
                    foreach (Beam x in Lista_Estacas)
                    {
                        linha = x.Name;
                        File.AppendAllText(filePath, linha + System.Environment.NewLine);
                        foreach (Combination y in Envoltoria)
                        {
                            linha = y.Name;
                            File.AppendAllText(filePath, linha + System.Environment.NewLine);

                            #region ESTACA

                            File.AppendAllText(filePath, linha_JV + System.Environment.NewLine);
                            File.AppendAllText(filePath_res, linha_JV + System.Environment.NewLine);
                            linha = "#Estaca: " + x.Name;
                            File.AppendAllText(filePath, linha + System.Environment.NewLine);
                            File.AppendAllText(filePath_res, linha + System.Environment.NewLine);
                            linha = "#Combinação: " + y.Name;
                            File.AppendAllText(filePath, linha + System.Environment.NewLine);
                            File.AppendAllText(filePath_res, linha + System.Environment.NewLine);
                            linha = "#Forças Internas";
                            File.AppendAllText(filePath, linha + System.Environment.NewLine);
                            File.AppendAllText(filePath_res, linha + System.Environment.NewLine);
                            File.AppendAllText(filePath, linha_JV + System.Environment.NewLine);
                            File.AppendAllText(filePath_res, linha_JV + System.Environment.NewLine);


                            #region FORÇA INTERNA
                            //Create container for 1D results
                            Result Results_InnerForces_estaca = new Result();
                            //Results key for internal forces on beam 1
                            ResultKey InnerForces_estaca = new ResultKey
                            {
                                EntityType = eDsElementType.eDsElementType_Beam,
                                EntityName = x.Name,
                                CaseType = eDsElementType.eDsElementType_Combination,
                                CaseId = y.Id,
                                Dimension = eDimension.eDim_1D,
                                ResultType = eResultType.eFemBeamInnerForces,
                                CoordSystem = eCoordSystem.eCoordSys_Local
                            };
                            Results_InnerForces_estaca = resultsApi.LoadResult(InnerForces_estaca);
                
                            linha = Results_InnerForces_estaca.GetTextOutput();
                            //Console.WriteLine(linha);
                            File.AppendAllText(filePath, linha + System.Environment.NewLine);
                            File.AppendAllText(filePath, linha_JV + System.Environment.NewLine);


                            //double[] pivot = new double[7];
                            //double[] maxvalue = new double[7];
                            //for (int j = 0; j <= 5; j++) { maxvalue[j] = 0; }

                            //for (int i = 0; i < Results_InnerForces_estaca.GetMeshElementCount(); i++)
                            //{
                            //    for (int j = 0; j <= 5; j++) {

                            //        pivot[j] = Results_InnerForces_estaca.GetValue(j, i);
                            //        if (System.Math.Abs(pivot[j]) > System.Math.Abs(maxvalue[j])) { maxvalue[j] = pivot[j]; }
                            //    }
                            //}
                            #region VALORES EXTREMOS
                            double nro_layers = Results_InnerForces_estaca.GetLayersCount();
                            double nro_mesh_elements = Results_InnerForces_estaca.GetMeshElementCount();
                            double nro_magnitudes = Results_InnerForces_estaca.GetMagnitudesCount();
                            double[] pivot = new double[7];
                            //double dados;
                            double[] maxvalue = new double[7];
                            double[] minvalue = new double[7];
                            double[] maxabsvalue = new double[7];
                            string linha_cab; string linha_dados; string linha_aux;
                            for (int j = 0; j < nro_magnitudes; j++) { maxvalue[j] = -1e50; minvalue[j] = 1e50; }

                            for (int i = 0; i < nro_magnitudes; i++)
                            {
                                for (int j = 0; j < nro_layers; j++)
                                {

                                    Results_InnerForces_estaca.SetActualLayer(j);

                                    for (int l = 0; l < nro_mesh_elements; l++)
                                    {
                                        pivot[i] = Results_InnerForces_estaca.GetValue(i, l);
                                        //dados = Results_InnerForces_estaca.GetValue(i, l);
                                        //Console.WriteLine(i + " " + j + " " + l + dados);
                                        if (pivot[i] < minvalue[i]) { minvalue[i] = pivot[i]; }
                                        if (pivot[i] > maxvalue[i]) { maxvalue[i] = pivot[i]; }
                                    }
                                }
                            }

                            linha_aux = "Maximum inner force ";
                            linha_aux = linha_aux.PadLeft(35, ' ');
                            linha_cab = "      " + linha_aux;
                            linha_aux = "Minimum inner force ";
                            linha_aux = linha_aux.PadLeft(35, ' ');
                            linha_cab += linha_aux;
                            linha_aux = "Maximum absolute inner force ";
                            linha_aux = linha_aux.PadLeft(35, ' ');
                            linha_cab += linha_aux;

                            File.AppendAllText(filePath_res, linha_cab + System.Environment.NewLine);

                            for (int j = 0; j < nro_magnitudes; j++)
                            {
                                if (Math.Abs(maxvalue[j]) > Math.Abs(minvalue[j]))
                                {
                                    maxabsvalue[j] = Math.Abs(maxvalue[j]);
                                }
                                else
                                {
                                    maxabsvalue[j] = Math.Abs(minvalue[j]);
                                }

                                Resultados resultado = new Resultados(x.Name, true, y.Name, true, Results_InnerForces_estaca.GetMagnitudeName(j), minvalue[j], maxvalue[j]);
                                Lista_resultados.Add(resultado);

                                linha_aux = Results_InnerForces_estaca.GetMagnitudeName(j); linha_aux = linha_aux.PadLeft(5, ' ');
                                linha_dados = linha_aux;

                                linha_aux = maxvalue[j].ToString(); linha_aux = linha_aux.PadLeft(35, ' ');
                                linha_dados += linha_aux;
                                linha_aux = minvalue[j].ToString(); linha_aux = linha_aux.PadLeft(35, ' ');
                                linha_dados += linha_aux;
                                linha_aux = maxabsvalue[j].ToString(); linha_aux = linha_aux.PadLeft(35, ' ');
                                linha_dados += linha_aux;


                                Console.WriteLine("Maximum inner force " + Results_InnerForces_estaca.GetMagnitudeName(j));
                                Console.WriteLine(maxvalue[j]);
                                Console.WriteLine("Minimum inner force " + Results_InnerForces_estaca.GetMagnitudeName(j));
                                Console.WriteLine(minvalue[j]);
                                Console.WriteLine("Maximum absolute inner force " + Results_InnerForces_estaca.GetMagnitudeName(j));
                                Console.WriteLine(maxabsvalue[j]);

                                File.AppendAllText(filePath, "Maximum inner force " + Results_InnerForces_estaca.GetMagnitudeName(j) + System.Environment.NewLine);
                                File.AppendAllText(filePath, maxvalue[j] + System.Environment.NewLine);
                                File.AppendAllText(filePath, "Minimum inner force " + Results_InnerForces_estaca.GetMagnitudeName(j) + System.Environment.NewLine);
                                File.AppendAllText(filePath, minvalue[j] + System.Environment.NewLine);
                                File.AppendAllText(filePath, "Maximum absolute inner force " + Results_InnerForces_estaca.GetMagnitudeName(j) + System.Environment.NewLine);
                                File.AppendAllText(filePath, maxabsvalue[j] + System.Environment.NewLine);

                                File.AppendAllText(filePath_res, linha_dados + System.Environment.NewLine);
                            }
                            #endregion
                            File.AppendAllText(filePath, linha_JV + System.Environment.NewLine);
                            linha = "#Deformações";
                            File.AppendAllText(filePath, linha + System.Environment.NewLine);
                            #endregion

                            #region DEFORMAÇÃO
                            //OpenApiE2EResult Deformations_estaca1C_ELU_1 = new OpenApiE2EResult("Deformations_estaca1C_ELU_1")
                            //Create container for 1D results
                            Result Results_Deformations_estaca = new Result();
                            //Results key for internal forces on beam 1
                            ResultKey Deformations_estaca = new ResultKey
                            {
                                EntityType = eDsElementType.eDsElementType_Beam,
                                EntityName = x.Name,
                                CaseType = eDsElementType.eDsElementType_Combination,
                                CaseId = y.Id,
                                Dimension = eDimension.eDim_1D,
                                ResultType = eResultType.eFemBeamDeformation,
                                CoordSystem = eCoordSystem.eCoordSys_Local
                            };
                            Results_Deformations_estaca = resultsApi.LoadResult(Deformations_estaca);

                            linha = Results_Deformations_estaca.GetTextOutput();
                            //Console.WriteLine(linha);
                            File.AppendAllText(filePath, linha + System.Environment.NewLine);
                            File.AppendAllText(filePath, linha_JV + System.Environment.NewLine);

                            //pivot = new double[7];
                            //maxvalue = new double[7];
                            //for (int j = 0; j <= 6; j++) { maxvalue[j] = 0; }

                            //for (int i = 0; i < Results_Deformations_estaca.GetMeshElementCount(); i++)
                            //{
                            //    for (int j = 0; j <= 6; j++)
                            //    {

                            //        pivot[j] = Results_Deformations_estaca.GetValue(j, i);
                            //        if (System.Math.Abs(pivot[j]) > System.Math.Abs(maxvalue[j])) { maxvalue[j] = pivot[j]; }
                            //    }
                            //}
                            //for (int j = 0; j <= 6; j++)
                            //{
                            //    Console.WriteLine("Maximum deformation " + j);
                            //    Console.WriteLine(maxvalue[j]);
                            //    File.AppendAllText(filePath, "Maximum deformation " + System.Environment.NewLine);
                            //    File.AppendAllText(filePath, maxvalue[j] + System.Environment.NewLine);
                            //}
                            #region VALORES EXTREMOS
                            nro_layers = Results_Deformations_estaca.GetLayersCount();
                            nro_mesh_elements = Results_Deformations_estaca.GetMeshElementCount();
                            nro_magnitudes = Results_Deformations_estaca.GetMagnitudesCount();
                            //double[] pivot = new double[7];
                            //double dados;
                            //double[] maxvalue = new double[7];
                            //double[] minvalue = new double[7];
                            //double[] maxabsvalue = new double[7];
                            for (int j = 0; j < nro_magnitudes; j++) { maxvalue[j] = -1e50; minvalue[j] = 1e50; }

                            for (int i = 0; i < nro_magnitudes; i++)
                            {
                                for (int j = 0; j < nro_layers; j++)
                                {

                                    Results_Deformations_estaca.SetActualLayer(j);

                                    for (int l = 0; l < nro_mesh_elements; l++)
                                    {
                                        pivot[i] = Results_Deformations_estaca.GetValue(i, l);
                                        //dados = Results_Deformations_estaca.GetValue(i, l);
                                        //Console.WriteLine(i + " " + j + " " + l + dados);
                                        if (pivot[i] < minvalue[i]) { minvalue[i] = pivot[i]; }
                                        if (pivot[i] > maxvalue[i]) { maxvalue[i] = pivot[i]; }
                                    }
                                }
                            }

                            File.AppendAllText(filePath_res, linha_JV + System.Environment.NewLine);
                            linha_aux = "#Deformações";
                            File.AppendAllText(filePath_res, linha_aux + System.Environment.NewLine);

                            linha_aux = "Maximum deformation ";
                            linha_aux = linha_aux.PadLeft(35, ' ');
                            linha_cab = "      " + linha_aux;
                            linha_aux = "Minimum deformation ";
                            linha_aux = linha_aux.PadLeft(35, ' ');
                            linha_cab += linha_aux;
                            linha_aux = "Maximum absolute deformation ";
                            linha_aux = linha_aux.PadLeft(35, ' ');
                            linha_cab += linha_aux;

                            File.AppendAllText(filePath_res, linha_JV + System.Environment.NewLine);
                            File.AppendAllText(filePath_res, linha_cab + System.Environment.NewLine);

                            for (int j = 0; j < nro_magnitudes; j++)
                            {
                                if (Math.Abs(maxvalue[j]) > Math.Abs(minvalue[j]))
                                {
                                    maxabsvalue[j] = Math.Abs(maxvalue[j]);
                                }
                                else
                                {
                                    maxabsvalue[j] = Math.Abs(minvalue[j]);
                                }

                                Resultados resultado = new Resultados(x.Name, true, y.Name, false, Results_Deformations_estaca.GetMagnitudeName(j), minvalue[j], maxvalue[j]);
                                Lista_resultados.Add(resultado);

                                linha_aux = Results_Deformations_estaca.GetMagnitudeName(j); linha_aux = linha_aux.PadLeft(5, ' ');
                                linha_dados = linha_aux;

                                linha_aux = maxvalue[j].ToString(); linha_aux = linha_aux.PadLeft(35, ' ');
                                linha_dados += linha_aux;
                                linha_aux = minvalue[j].ToString(); linha_aux = linha_aux.PadLeft(35, ' ');
                                linha_dados += linha_aux;
                                linha_aux = maxabsvalue[j].ToString(); linha_aux = linha_aux.PadLeft(35, ' ');
                                linha_dados += linha_aux;


                                Console.WriteLine("Maximum deformation " + Results_Deformations_estaca.GetMagnitudeName(j));
                                Console.WriteLine(maxvalue[j]);
                                Console.WriteLine("Minimum deformation " + Results_Deformations_estaca.GetMagnitudeName(j));
                                Console.WriteLine(minvalue[j]);
                                Console.WriteLine("Maximum absolute deformation " + Results_Deformations_estaca.GetMagnitudeName(j));
                                Console.WriteLine(maxabsvalue[j]);

                                File.AppendAllText(filePath, "Maximum deformation " + Results_Deformations_estaca.GetMagnitudeName(j) + System.Environment.NewLine);
                                File.AppendAllText(filePath, maxvalue[j] + System.Environment.NewLine);
                                File.AppendAllText(filePath, "Minimum deformation " + Results_Deformations_estaca.GetMagnitudeName(j) + System.Environment.NewLine);
                                File.AppendAllText(filePath, minvalue[j] + System.Environment.NewLine);
                                File.AppendAllText(filePath, "Maximum absolute deformation " + Results_Deformations_estaca.GetMagnitudeName(j) + System.Environment.NewLine);
                                File.AppendAllText(filePath, maxabsvalue[j] + System.Environment.NewLine);

                                File.AppendAllText(filePath_res, linha_dados + System.Environment.NewLine);
                            }
                            #endregion
                            #endregion

                            #endregion
                        }
                    }
                    #endregion

                    #region LAJE

                    linha = slab.Name;
                    File.AppendAllText(filePath, linha + System.Environment.NewLine);
                    foreach (Combination y in Envoltoria)
                    {
                        linha = y.Name;
                        File.AppendAllText(filePath, linha + System.Environment.NewLine);

                        #region LAJE

                        File.AppendAllText(filePath, linha_JV + System.Environment.NewLine);
                        File.AppendAllText(filePath_res, linha_JV + System.Environment.NewLine);
                        linha = "#Laje: " + slab.Name;
                        File.AppendAllText(filePath, linha + System.Environment.NewLine);
                        File.AppendAllText(filePath_res, linha + System.Environment.NewLine);
                        linha = "#Combinação: " + y.Name;
                        File.AppendAllText(filePath, linha + System.Environment.NewLine);
                        File.AppendAllText(filePath_res, linha + System.Environment.NewLine);

                        linha = "#Forças Internas";
                        File.AppendAllText(filePath, linha + System.Environment.NewLine);
                        File.AppendAllText(filePath_res, linha + System.Environment.NewLine);
                        File.AppendAllText(filePath, linha_JV + System.Environment.NewLine);
                        File.AppendAllText(filePath_res, linha_JV + System.Environment.NewLine);




                        #region FORÇA INTERNA
                        //Create container for 2D results
                        Result Results_InnerForces_slab = new Result();
                        //Results key for internal forces on slab 1
                        ResultKey InnerForces_slab = new ResultKey
                        {
                            EntityType = eDsElementType.eDsElementType_Slab,
                            EntityName = slab.Name,
                            CaseType = eDsElementType.eDsElementType_Combination,
                            CaseId = y.Id,
                            Dimension = eDimension.eDim_2D,
                            ResultType = eResultType.eFemInnerForces,
                            CoordSystem = eCoordSystem.eCoordSys_Local
                        };
                        Results_InnerForces_slab = resultsApi.LoadResult(InnerForces_slab);

                        linha = Results_InnerForces_slab.GetTextOutput();
                        //Console.WriteLine(linha);
                        File.AppendAllText(filePath, linha + System.Environment.NewLine);
                        File.AppendAllText(filePath, linha_JV + System.Environment.NewLine);


                        //double[] pivot = new double[8];
                        //double[] maxvalue = new double[8];
                        //for (int j = 0; j <= 7; j++) { maxvalue[j] = 0; }

                        //for (int i = 0; i < Results_InnerForces_slab.GetMeshElementCount(); i++)
                        //{
                        //    for (int j = 0; j <= 7; j++)
                        //    {

                        //        pivot[j] = Results_InnerForces_slab.GetValue(j, i);
                        //        if (System.Math.Abs(pivot[j]) > System.Math.Abs(maxvalue[j])) { maxvalue[j] = pivot[j]; }
                        //    }
                        //}
                        //for (int j = 0; j <= 7; j++)
                        //{
                        //    Console.WriteLine("Maximum inner force " + j);
                        //    Console.WriteLine(maxvalue[j]);
                        //    File.AppendAllText(filePath, "Maximum inner force " + System.Environment.NewLine);
                        //    File.AppendAllText(filePath, maxvalue[j] + System.Environment.NewLine);
                        //}

                        #region VALORES EXTREMOS
                        double nro_layers = Results_InnerForces_slab.GetLayersCount();
                        double nro_mesh_elements = Results_InnerForces_slab.GetMeshElementCount();
                        double nro_magnitudes = Results_InnerForces_slab.GetMagnitudesCount();
                        double[] pivot = new double[8];
                        //double dados;
                        double[] maxvalue = new double[8];
                        double[] minvalue = new double[8];
                        double[] maxabsvalue = new double[8];
                        string linha_cab; string linha_dados; string linha_aux;
                        for (int j = 0; j < nro_magnitudes; j++) { maxvalue[j] = -1e50; minvalue[j] = 1e50; }

                        for (int i = 0; i < nro_magnitudes; i++)
                        {
                            for (int j = 0; j < nro_layers; j++)
                            {

                                Results_InnerForces_slab.SetActualLayer(j);

                                for (int l = 0; l < nro_mesh_elements; l++)
                                {
                                    pivot[i] = Results_InnerForces_slab.GetValue(i, l);
                                    //dados = Results_InnerForces_slab.GetValue(i, l);
                                    //Console.WriteLine(i + " " + j + " " + l + dados);
                                    if (pivot[i] < minvalue[i]) { minvalue[i] = pivot[i]; }
                                    if (pivot[i] > maxvalue[i]) { maxvalue[i] = pivot[i]; }
                                }
                            }
                        }

                        linha_aux = "Maximum inner force ";
                        linha_aux = linha_aux.PadLeft(35, ' ');
                        linha_cab = "      " + linha_aux;
                        linha_aux = "Minimum inner force ";
                        linha_aux = linha_aux.PadLeft(35, ' ');
                        linha_cab += linha_aux;
                        linha_aux = "Maximum absolute inner force ";
                        linha_aux = linha_aux.PadLeft(35, ' ');
                        linha_cab += linha_aux;

                        File.AppendAllText(filePath_res, linha_cab + System.Environment.NewLine);


                        for (int j = 0; j < nro_magnitudes; j++)
                        {
                            if (Math.Abs(maxvalue[j]) > Math.Abs(minvalue[j]))
                            {
                                maxabsvalue[j] = Math.Abs(maxvalue[j]);
                            }
                            else
                            {
                                maxabsvalue[j] = Math.Abs(minvalue[j]);
                            }

                            Resultados resultado = new Resultados(slab.Name, false, y.Name, true, Results_InnerForces_slab.GetMagnitudeName(j), minvalue[j], maxvalue[j]);
                            Lista_resultados.Add(resultado);

                            linha_aux = Results_InnerForces_slab.GetMagnitudeName(j); linha_aux = linha_aux.PadLeft(5, ' ');
                            linha_dados = linha_aux;

                            linha_aux = maxvalue[j].ToString(); linha_aux = linha_aux.PadLeft(35, ' ');
                            linha_dados += linha_aux;
                            linha_aux = minvalue[j].ToString(); linha_aux = linha_aux.PadLeft(35, ' ');
                            linha_dados += linha_aux;
                            linha_aux = maxabsvalue[j].ToString(); linha_aux = linha_aux.PadLeft(35, ' ');
                            linha_dados += linha_aux;
                        

                            Console.WriteLine("Maximum inner force " + Results_InnerForces_slab.GetMagnitudeName(j));
                            Console.WriteLine(maxvalue[j]);
                            Console.WriteLine("Minimum inner force " + Results_InnerForces_slab.GetMagnitudeName(j));
                            Console.WriteLine(minvalue[j]);
                            Console.WriteLine("Maximum absolute inner force " + Results_InnerForces_slab.GetMagnitudeName(j));
                            Console.WriteLine(maxabsvalue[j]);

                            File.AppendAllText(filePath, "Maximum inner force " + Results_InnerForces_slab.GetMagnitudeName(j) + System.Environment.NewLine);
                            File.AppendAllText(filePath, maxvalue[j] + System.Environment.NewLine);
                            File.AppendAllText(filePath, "Minimum inner force " + Results_InnerForces_slab.GetMagnitudeName(j) + System.Environment.NewLine);
                            File.AppendAllText(filePath, minvalue[j] + System.Environment.NewLine);
                            File.AppendAllText(filePath, "Maximum absolute inner force " + Results_InnerForces_slab.GetMagnitudeName(j) + System.Environment.NewLine);
                            File.AppendAllText(filePath, maxabsvalue[j] + System.Environment.NewLine);

                            File.AppendAllText(filePath_res, linha_dados + System.Environment.NewLine);


                        }
                        #endregion
                        #endregion

                        #region DEFORMAÇÃO

                        File.AppendAllText(filePath, linha_JV + System.Environment.NewLine);
                        linha = "#Deformações";
                        File.AppendAllText(filePath, linha + System.Environment.NewLine);
                    
                        //OpenApiE2EResult Deformations_estaca1C_ELU_1 = new OpenApiE2EResult("Deformations_estaca1C_ELU_1")
                        //Create container for 1D results
                        Result Results_Deformations_slab = new Result();
                        //Results key for internal forces on beam 1
                        ResultKey Deformations_slab = new ResultKey
                        {
                            EntityType = eDsElementType.eDsElementType_Slab,
                            EntityName = slab.Name,
                            CaseType = eDsElementType.eDsElementType_Combination,
                            CaseId = y.Id,
                            Dimension = eDimension.eDim_2D,
                            ResultType = eResultType.eFemDeformations,
                            CoordSystem = eCoordSystem.eCoordSys_Local
                        };
                        Results_Deformations_slab = resultsApi.LoadResult(Deformations_slab);

                        linha = Results_Deformations_slab.GetTextOutput();
                        //Console.WriteLine(linha);
                        File.AppendAllText(filePath, linha + System.Environment.NewLine);
                        File.AppendAllText(filePath, linha_JV + System.Environment.NewLine);

                        //pivot = new double[7];
                        //maxvalue = new double[7];
                        //for (int j = 0; j <= 6; j++) { maxvalue[j] = 0; }
                        //Console.WriteLine(Results_Deformations_slab.GetMeshElementCount());

                        //for (int i = 0; i < Results_Deformations_slab.GetMeshElementCount(); i++)
                        //{
                        //    for (int j = 0; j <= 6; j++)
                        //    {

                        //        pivot[j] = Results_Deformations_slab.GetValue(j, i);
                        //        if (System.Math.Abs(pivot[j]) > System.Math.Abs(maxvalue[j])) { maxvalue[j] = pivot[j]; }
                        //    }
                        //}
                        //for (int j = 0; j <= 6; j++)
                        //{
                        //    Console.WriteLine("Maximum deformation " + j);
                        //    Console.WriteLine(maxvalue[j]);
                        //    File.AppendAllText(filePath, "Maximum deformation " + System.Environment.NewLine);
                        //    File.AppendAllText(filePath, maxvalue[j] + System.Environment.NewLine);
                        //}

                        #region VALORES EXTREMOS
                        nro_layers = Results_Deformations_slab.GetLayersCount();
                        nro_mesh_elements = Results_Deformations_slab.GetMeshElementCount();
                        nro_magnitudes = Results_Deformations_slab.GetMagnitudesCount();
                        //double[] pivot = new double[7];
                        //double dados;
                        //double[] maxvalue = new double[7];
                        //double[] minvalue = new double[7];
                        //double[] maxabsvalue = new double[7];
                        for (int j = 0; j < nro_magnitudes; j++) { maxvalue[j] = -1e50; minvalue[j] = 1e50; }

                        for (int i = 0; i < nro_magnitudes; i++)
                        {
                            for (int j = 0; j < nro_layers; j++)
                            {

                                Results_Deformations_slab.SetActualLayer(j);

                                for (int l = 0; l < nro_mesh_elements; l++)
                                {
                                    pivot[i] = Results_Deformations_slab.GetValue(i, l);
                                    //dados = Results_Deformations_slab.GetValue(i, l);
                                    //Console.WriteLine(i + " " + j + " " + l + dados);
                                    if (pivot[i] < minvalue[i]) { minvalue[i] = pivot[i]; }
                                    if (pivot[i] > maxvalue[i]) { maxvalue[i] = pivot[i]; }
                                }
                            }
                        }

                        File.AppendAllText(filePath_res, linha_JV + System.Environment.NewLine);
                        linha_aux = "#Deformações";
                        File.AppendAllText(filePath_res, linha_aux + System.Environment.NewLine);

                        linha_aux = "Maximum deformation ";
                        linha_aux = linha_aux.PadLeft(35, ' ');
                        linha_cab = "      " + linha_aux;
                        linha_aux = "Minimum deformation ";
                        linha_aux = linha_aux.PadLeft(35, ' ');
                        linha_cab += linha_aux;
                        linha_aux = "Maximum absolute deformation ";
                        linha_aux = linha_aux.PadLeft(35, ' ');
                        linha_cab += linha_aux;

                        File.AppendAllText(filePath_res, linha_JV + System.Environment.NewLine);
                        File.AppendAllText(filePath_res, linha_cab + System.Environment.NewLine);

                        for (int j = 0; j < nro_magnitudes; j++)
                        {
                            if (Math.Abs(maxvalue[j]) > Math.Abs(minvalue[j]))
                            {
                                maxabsvalue[j] = Math.Abs(maxvalue[j]);
                            }
                            else
                            {
                                maxabsvalue[j] = Math.Abs(minvalue[j]);
                            }

                            Resultados resultado = new Resultados(slab.Name, false, y.Name, false, Results_Deformations_slab.GetMagnitudeName(j), minvalue[j], maxvalue[j]);
                            Lista_resultados.Add(resultado);

                            linha_aux = Results_Deformations_slab.GetMagnitudeName(j); linha_aux = linha_aux.PadLeft(5, ' ');
                            linha_dados = linha_aux;

                            linha_aux = maxvalue[j].ToString(); linha_aux = linha_aux.PadLeft(35, ' ');
                            linha_dados += linha_aux;
                            linha_aux = minvalue[j].ToString(); linha_aux = linha_aux.PadLeft(35, ' ');
                            linha_dados += linha_aux;
                            linha_aux = maxabsvalue[j].ToString(); linha_aux = linha_aux.PadLeft(35, ' ');
                            linha_dados += linha_aux;


                            Console.WriteLine("Maximum deformation " + Results_Deformations_slab.GetMagnitudeName(j));
                            Console.WriteLine(maxvalue[j]);
                            Console.WriteLine("Minimum deformation " + Results_Deformations_slab.GetMagnitudeName(j));
                            Console.WriteLine(minvalue[j]);
                            Console.WriteLine("Maximum absolute deformation " + Results_Deformations_slab.GetMagnitudeName(j));
                            Console.WriteLine(maxabsvalue[j]);

                            File.AppendAllText(filePath, "Maximum deformation " + Results_Deformations_slab.GetMagnitudeName(j) + System.Environment.NewLine);
                            File.AppendAllText(filePath, maxvalue[j] + System.Environment.NewLine);
                            File.AppendAllText(filePath, "Minimum deformation " + Results_Deformations_slab.GetMagnitudeName(j) + System.Environment.NewLine);
                            File.AppendAllText(filePath, minvalue[j] + System.Environment.NewLine);
                            File.AppendAllText(filePath, "Maximum absolute deformation " + Results_Deformations_slab.GetMagnitudeName(j) + System.Environment.NewLine);
                            File.AppendAllText(filePath, maxabsvalue[j] + System.Environment.NewLine);

                            File.AppendAllText(filePath_res, linha_dados + System.Environment.NewLine);


                        }
                        #endregion
                        #endregion

                        #endregion


                    }
                        #endregion

                }

                #endregion
                double[,,,] pivot_res = new double[2, 4, 2, 8];
                double[,,,] matriz_max = new double[2, 4, 2, 8];
                double[,,,] matriz_min = new double[2, 4, 2, 8];
                double[,,,] matriz_abs = new double[2, 4, 2, 8];
                List<Resultados_el_env_FiD_mag> lista_dados = new List<Resultados_el_env_FiD_mag> { };
                List<Resultados_el_env_FiD_mag> lista_dados_res = new List<Resultados_el_env_FiD_mag> { };

                foreach (Resultados var_resultado in Lista_resultados)
                {
                    Resultados_el_env_FiD_mag dados = new Resultados_el_env_FiD_mag(var_resultado.Categoria(), var_resultado.Minimo, var_resultado.Maximo);
                    
                    lista_dados.Add(dados);
                }

                lista_dados.Sort((x, y) => x.Categoria.CompareTo(y.Categoria));

                string categoria_atu; string categoria_ant = "";
                double pivot_max = -1e50; double pivot_min = 1e50;
                k = 0;
                foreach(Resultados_el_env_FiD_mag dados in lista_dados)
                {
                    categoria_atu = dados.Categoria;
                    //if (k == 0)
                    //{
                    //    pivot_max = dados.Maximo;
                    //    pivot_min = dados.Minimo;
                    //    categoria_ant = categoria_atu;
                    //}
                    if(categoria_atu == categoria_ant)
                    {
                        if (dados.Maximo > pivot_max)
                        {
                            pivot_max = dados.Maximo;
                        }
                        if (dados.Minimo < pivot_min)
                        {
                            pivot_min = dados.Minimo;
                        }
                    }
                    else
                    {
                        Resultados_el_env_FiD_mag dados1 = new Resultados_el_env_FiD_mag(categoria_ant, pivot_min, pivot_max);
                        if (categoria_ant != "") { lista_dados_res.Add(dados1); }
                        pivot_max = dados.Maximo;
                        pivot_min = dados.Minimo;
                        
                    }

                    categoria_ant = categoria_atu;

                    Console.WriteLine(dados.Categoria+" "+ dados.Minimo + " " + dados.Maximo);
                }
                
                Console.WriteLine(linha_JV);

                foreach (Resultados_el_env_FiD_mag dados in lista_dados_res)
                {
                    linha = dados.Categoria + " " + dados.Minimo.ToString("0.#####").PadLeft(22) + " " + dados.Maximo.ToString("0.#####").PadLeft(22);
                    Console.WriteLine(linha);
                    File.AppendAllText(filePath_res_cons, linha + System.Environment.NewLine);

                }
                
                #region ENROLADO
                //OpenApiE2EResult InnerForces_estaca1C_ELU_1 = new OpenApiE2EResult("InnerForces_estaca1C_ELU_1")
                //{
                //    ResultKey = new ResultKey
                //    {
                //        EntityType = eDsElementType.eDsElementType_Beam,
                //        EntityName = estaca1C.Name,
                //        CaseType = eDsElementType.eDsElementType_Combination,
                //        CaseId = ELU_1.Id,
                //        Dimension = eDimension.eDim_1D,
                //        ResultType = eResultType.eFemBeamInnerForces,
                //        CoordSystem = eCoordSystem.eCoordSys_Local
                //    }
                //};
                //InnerForces_estaca1C_ELU_1.Result = resultsApi.LoadResult(InnerForces_estaca1C_ELU_1.ResultKey);
                //storage.SetResult(InnerForces_estaca1C_ELU_1);
                ////}
                //{
                //    OpenApiE2EResult Deformations_estaca1C_ELU_1 = new OpenApiE2EResult("Deformations_estaca1C_ELU_1")
                //    {
                //        ResultKey = new ResultKey
                //        {
                //            EntityType = eDsElementType.eDsElementType_Beam,
                //            EntityName = estaca1C.Name,
                //            CaseType = eDsElementType.eDsElementType_Combination,
                //            CaseId = ELU_1.Id,
                //            Dimension = eDimension.eDim_1D,
                //            ResultType = eResultType.eFemBeamDeformation,
                //            CoordSystem = eCoordSystem.eCoordSys_Local
                //        }
                //    };
                //    Deformations_estaca1C_ELU_1.Result = resultsApi.LoadResult(Deformations_estaca1C_ELU_1.ResultKey);
                //    storage.SetResult(Deformations_estaca1C_ELU_1);
                //}
                //{
                //    OpenApiE2EResult beamB1RelIDeformationLc = new OpenApiE2EResult("beamB1RelativeDeformationsLC1")
                //    {
                //        ResultKey = new ResultKey
                //        {
                //            EntityType = eDsElementType.eDsElementType_Beam,
                //            EntityName = beamName,
                //            CaseType = eDsElementType.eDsElementType_LoadCase,
                //            CaseId = Lc1Id,
                //            Dimension = eDimension.eDim_1D,
                //            ResultType = eResultType.eFemBeamRelativeDeformation,
                //            CoordSystem = eCoordSystem.eCoordSys_Local
                //        }
                //    };
                //    beamB1RelIDeformationLc.Result = resultsApi.LoadResult(beamB1RelIDeformationLc.ResultKey);
                //    storage.SetResult(beamB1RelIDeformationLc);
                //}
                //{
                //    OpenApiE2EResult beamInnerForcesCombi = new OpenApiE2EResult("beamInnerForcesCombi")
                //    {
                //        ResultKey = new ResultKey
                //        {
                //            EntityType = eDsElementType.eDsElementType_Beam,
                //            EntityName = beamName,
                //            CaseType = eDsElementType.eDsElementType_Combination,
                //            CaseId = C1Id,
                //            Dimension = eDimension.eDim_1D,
                //            ResultType = eResultType.eFemBeamInnerForces,
                //            CoordSystem = eCoordSystem.eCoordSys_Local
                //        }
                //    };
                //    beamInnerForcesCombi.Result = resultsApi.LoadResult(beamInnerForcesCombi.ResultKey);
                //    storage.SetResult(beamInnerForcesCombi);
                //}


                //{
                //    OpenApiE2EResult slabInnerForces = new OpenApiE2EResult("slabInnerForces")
                //    {
                //        ResultKey = new ResultKey
                //        {
                //            EntityType = eDsElementType.eDsElementType_Slab,
                //            EntityName = SlabName,
                //            CaseType = eDsElementType.eDsElementType_LoadCase,
                //            CaseId = Lc1Id,
                //            Dimension = eDimension.eDim_2D,
                //            ResultType = eResultType.eFemInnerForces,
                //            CoordSystem = eCoordSystem.eCoordSys_Local
                //        }
                //    };
                //    slabInnerForces.Result = resultsApi.LoadResult(slabInnerForces.ResultKey);
                //    storage.SetResult(slabInnerForces);
                //}
                //{
                //    OpenApiE2EResult slabDeformations = new OpenApiE2EResult("slabDeformations")
                //    {
                //        ResultKey = new ResultKey
                //        {
                //            EntityType = eDsElementType.eDsElementType_Slab,
                //            EntityName = SlabName,
                //            CaseType = eDsElementType.eDsElementType_LoadCase,
                //            CaseId = Lc1Id,
                //            Dimension = eDimension.eDim_2D,
                //            ResultType = eResultType.eFemDeformations,
                //            CoordSystem = eCoordSystem.eCoordSys_Local
                //        }
                //    };
                //    slabDeformations.Result = resultsApi.LoadResult(slabDeformations.ResultKey);
                //    storage.SetResult(slabDeformations);
                //}
                //{
                //    OpenApiE2EResult slabStresses = new OpenApiE2EResult("slabStresses")
                //    {
                //        ResultKey = new ResultKey
                //        {
                //            EntityType = eDsElementType.eDsElementType_Slab,
                //            EntityName = SlabName,
                //            CaseType = eDsElementType.eDsElementType_LoadCase,
                //            CaseId = Lc1Id,
                //            Dimension = eDimension.eDim_2D,
                //            ResultType = eResultType.eFemStress,
                //            CoordSystem = eCoordSystem.eCoordSys_Local
                //        }
                //    };
                //    slabStresses.Result = resultsApi.LoadResult(slabStresses.ResultKey);
                //    storage.SetResult(slabStresses);
                //}
                //{
                //    OpenApiE2EResult slabStrains = new OpenApiE2EResult("slabStrains")
                //    {
                //        ResultKey = new ResultKey
                //        {
                //            EntityType = eDsElementType.eDsElementType_Slab,
                //            EntityName = SlabName,
                //            CaseType = eDsElementType.eDsElementType_LoadCase,
                //            CaseId = Lc1Id,
                //            Dimension = eDimension.eDim_2D,
                //            ResultType = eResultType.eFemStrains,
                //            CoordSystem = eCoordSystem.eCoordSys_Local
                //        }
                //    };
                //    slabStrains.Result = resultsApi.LoadResult(slabStrains.ResultKey);
                //    storage.SetResult(slabStrains);
                //}
                //{
                //    OpenApiE2EResult slabInnerForcesExtended = new OpenApiE2EResult("slabInnerForcesExtended")
                //    {
                //        ResultKey = new ResultKey
                //        {
                //            EntityType = eDsElementType.eDsElementType_Slab,
                //            EntityName = SlabName,
                //            CaseType = eDsElementType.eDsElementType_LoadCase,
                //            CaseId = Lc1Id,
                //            Dimension = eDimension.eDim_2D,
                //            ResultType = eResultType.eFemInnerForces_Extended,
                //            CoordSystem = eCoordSystem.eCoordSys_Local
                //        }
                //    };
                //    slabInnerForcesExtended.Result = resultsApi.LoadResult(slabInnerForcesExtended.ResultKey);
                //    storage.SetResult(slabInnerForcesExtended);
                //}
                #endregion
            }
            #endregion
            
            senData.Project.Model.RefreshModel_ToSCIAEngineer();
            senData.Project.CloseProject(SaveMode.SaveChangesYes);
            senData.Environment.Dispose();
            Tools.KillAllOrphanSCIAEngineerIntances();
            Thread.Sleep(5000);

            return;
        }
       
        public static void CreateCustomModel(Variaveis variaveis,[Optional] Variaveis_iter variaveis_iter)
        {
            //(Environment Environment, EsaProject Project) senData = Tools.StartSciaEngineer(SenInstallationPath, SenTempFolder, SenEmptyProject);
            //Variaveis var1 = variaveis;
            if (variaveis_iter.Equals(null))
            {
                CreateModelUsingOpenApi(variaveis);
                //CreateModelUsingOpenApi(senData.Project, variaveis);
                //senData.Project.Model.RefreshModel_ToSCIAEngineer();
                //senData.Project.CloseProject(SaveMode.SaveChangesYes);
                //senData.Environment.Dispose();
            }
            else
            {
                for (int i = 0; i<variaveis_iter.Estaca_alpha_iter; i++)
                {
                    double alpha1 = variaveis.Estaca_alpha + i * variaveis_iter.Estaca_alpha_passo;
                    for (int j = 0; j < variaveis_iter.Estaca_beta_iter; j++)
                    {
                        double beta1 = variaveis.Estaca_beta + j * variaveis_iter.Estaca_beta_passo;
                        for (int k = 0; k < variaveis_iter.No_estaca_x_iter; k++)
                        {
                            double no_estaca_x1 = variaveis.No_estaca_x + k * variaveis_iter.No_estaca_x_passo;
                            for (int l = 0; l < variaveis_iter.No_estaca_y_iter; l++)
                            {
                                double no_estaca_y1 = variaveis.No_estaca_y + l * variaveis_iter.No_estaca_y_passo;
                                Variaveis var1 = new Variaveis(variaveis.A, variaveis.B, variaveis.C, alpha1, beta1, variaveis.H_solo, variaveis.H_laje_solo, variaveis.H_agua, variaveis.H_conc, no_estaca_x1, no_estaca_y1, variaveis.Rigidez_i, variaveis.Rigidez_f, variaveis.No_cabeco_x, variaveis.No_cabeco_y, variaveis.Sobrecarga, variaveis.Corrente_long, variaveis.Corrente_transv, variaveis.Forca_cab);
                                CreateModelUsingOpenApi(var1);
                                //CreateModelUsingOpenApi(senData.Project, var1);
                                //senData.Project.Model.RefreshModel_ToSCIAEngineer();
                                //senData.Project.CloseProject(SaveMode.SaveChangesYes);
                                //senData.Environment.Dispose();
                            }

                        }
                    }
                }
            }
            //CreateModelUsingOpenApi(senData.Project, variaveis);
            //senData.Project.Model.RefreshModel_ToSCIAEngineer();
            //senData.Project.CloseProject(SaveMode.SaveChangesYes);
            //senData.Environment.Dispose();
        }
    }
}

































//private static void CreateCustomModel()
//{
//    var model = new AnalysisModel();
//    var admBootstrapper = new AnalysisDataModelBootstrapper();
//    using (var scope = admBootstrapper.CreateThreadedScope())
//    {
//        CreateModelUsingADM(model, scope.GetService<IAnalysisModelService>());
//    }

//    if (model.Count == 0)
//    {
//        Console.WriteLine("No items have been added to the model!");
//        return;
//    }
//    (Environment Environment, EsaProject Project) senData = Tools.StartSciaEngineer(SenInstallationPath, SenTempFolder, SenEmptyProject);

//    foreach (IAnalysisObject admObject in model)
//    {
//        senData.Project.Model.CreateAdmObject(admObject);
//    }

//    senData.Project.Model.RefreshModel_ToSCIAEngineer();
//    senData.Project.CloseProject(SaveMode.SaveChangesNo);

//    senData.Environment.Dispose();


//}
//    }
//}
