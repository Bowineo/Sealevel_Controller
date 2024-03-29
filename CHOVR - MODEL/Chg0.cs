﻿using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CHOV
{
    public class SystemSoftware : Functions
    {
        /// <summary>
        /// Função para receber e converter 2 strings em um vetor bool 16 posições [auxiliar para 'Slot']
        /// </summary>
        /// <param name="ReadPrimary"> String de entrada para ser convertida e aglutinada em um vetor bool de 16 posições</param>
        /// <param name="ReadPrimary1"> String de entrada para ser convertida e aglutinada em um vetor bool de 16 posições</param>
        /// <returns> Retorna um vetor bool juntando as 2 entradas string e convertendo para um vetor bool de 16 posições</returns>
        public static bool[] Bool_16position(string ReadPrimary, string ReadPrimary1)
        {
            bool[] VreadPrimary1 = VetorBoolean(FormatByte(ReadPrimary));
            bool[] VreadPrimary2 = VetorBoolean(FormatByte(ReadPrimary1));
            bool[] VreadPrimary = new bool[32];
            VreadPrimary1.CopyTo(VreadPrimary, 0);
            VreadPrimary2.CopyTo(VreadPrimary, 8);
            return VreadPrimary;
        }

        /// <summary>
        /// Função para analisar e executar as operações
        /// </summary>
        /// <param name="combinacoes">String collection com as combinações salvas no sistema</param>
        /// <param name="readPrimary">String com o valor decimal da leitura do primary 1-8</param>
        /// <param name="readPrimary1">String com o valor decimal da leitura do primary 9-16</param>
        /// <param name="readSecondary">String com o valor decimal da leitura do secondary 1-8</param>
        /// <param name="readSecondary1">String com o valor decimal da leitura do secondary 9-16</param>
        /// <returns>Vetor booleano com o status das combinações para escrever nas respectivas saidas</returns>
        public static bool[] ExecuteCombinations(System.Collections.Specialized.StringCollection combinations, string readPrimary, string readPrimary1, string readSecondary, string readSecondary1)
        {
            bool[] VreadPrimary = Bool_16position(readPrimary, readPrimary1);
            bool[] VreadSecondary = Bool_16position(readSecondary, readSecondary1);
            bool[] SlotV1 = new bool[18];
            bool[] SlotV2 = new bool[18];
            bool[] SlotT1 = new bool[18];
            bool[] SlotT2 = new bool[18];
            bool[] Z1 = new bool[18];
            bool[] Z2 = new bool[18];
            int t = combinations.Count;
            string[] device1 = new string[t];
            string[] name1 = new string[t];
            int[] input1 = new int[t];
            int[] in1_isNot = new int[t];
            string[] operacao = new string[t];
            string[] device2 = new string[t];
            string[] name2 = new string[t];
            int[] input2 = new int[t];
            int[] in2_isNot = new int[t];
            string[] nameoutput = new string[t];
            int[] output = new int[t];
            _ = new string[t];

            for (int i = 0; i < t; i++)
            {
                string[] comb = DiscreetCombination(combinations[i]);
                device1[i] = comb[0];
                in1_isNot[i] = InputStringToInt_not(comb[1]);
                input1[i] = Math.Abs(ZeroNegative(InputStringToInt_not(comb[1])));
                name1[i] = comb[2];
                operacao[i] = comb[3];
                device2[i] = comb[4];
                in2_isNot[i] = InputStringToInt_not(comb[5]);
                input2[i] = Math.Abs(ZeroNegative(InputStringToInt_not(comb[5])));
                name2[i] = comb[6];
                nameoutput[i] = comb[8];
                output[i] = OutputStringToInt(comb[7]);
            }
            //Vetor para saida do sistema com a resposta das operação direto p saida do hardware.
            bool[] VwriteOutput = new bool[18];
            for (int i = 0; i < t; i++)
            {
                if (in1_isNot[i] < 0)
                {
                    if (device1[i] == "Primary  ")
                    { SlotV1[i] = OperationNOT(VreadPrimary[input1[i]]); }
                    else
                    {
                        if (device1[i] == "- null -")
                        { SlotV1[i] = false; }
                        else { SlotV2[i] = OperationNOT(VreadSecondary[input1[i]]); }
                    }
                }
                else
                {
                    if (device1[i] == "Primary  ")
                    { SlotV1[i] = VreadPrimary[input1[i]]; }
                    else
                    {
                        if (device1[i] == "- null -")
                        { SlotV1[i] = false; }
                        else { SlotV2[i] = VreadSecondary[input1[i]]; }
                    }
                }
                if (in2_isNot[i] < 0)
                {
                    if (device2[i] == "Primary  ")
                    { SlotT1[i] = OperationNOT(VreadPrimary[input2[i]]); }
                    else
                    {
                        if (device2[i] == "- null -")
                        { SlotT2[i] = false; }
                        else { SlotT2[i] = OperationNOT(VreadSecondary[input2[i]]); }
                    }
                }
                else
                {
                    if (device2[i] == "Primary  ")
                    { SlotT1[i] = VreadPrimary[input2[i]]; }
                    else
                    {
                        if (device2[i] == "- null -")
                        { SlotT2[i] = false; }
                        else { SlotT2[i] = VreadSecondary[input2[i]]; }
                    }

                }
            }
            for (int i = 0; i < t; i++)
            {
                if (IsAnd(operacao[i]))
                {
                    if (device1[i] == "Primary  ") { Z1[i] = SlotV1[i]; }
                    else
                    {
                        if (device1[i] == "- null -") { Z1[i] = false; }
                        else { Z1[i] = SlotV2[i]; }
                    }
                    if (device2[i] == "Primary  ") { Z2[i] = SlotT1[i]; }
                    else
                    {
                        if (device2[i] == "- null -") { Z2[i] = false; }
                        else { Z2[i] = SlotT2[i]; }
                    }
                    VwriteOutput[output[i]] = OperationOR(VwriteOutput[output[i]], (OperationAND(Z1[i], Z2[i])));
                }
                else
                {
                    if (device1[i] == "Primary  ") { Z1[i] = SlotV1[i]; }
                    else
                    {
                        if (device1[i] == "- null -") { Z1[i] = false; }
                        else { Z1[i] = SlotV2[i]; }
                    }
                    if (device2[i] == "Primary  ") { Z2[i] = SlotT1[i]; }
                    else
                    {
                        if (device2[i] == "- null -") { Z2[i] = false; }
                        else { Z2[i] = SlotT2[i]; }
                    }
                    VwriteOutput[output[i]] = OperationOR(VwriteOutput[output[i]], (OperationOR(Z1[i], Z2[i])));
                }
            }
            return VwriteOutput;
        }

        /// <summary>
        /// Função para indentificar de acordo com a posição o 'name' Input
        /// </summary>
        /// <param name="n1"> StringCollection[] de input onde será procurado determinado 'name'</param>
        /// <param name="name"> String 'name' a ser procurado no StringCollection[]</param>
        /// <returns> Retorna string com a posição do name de input</returns>
        public static string PositionIndexIN(System.Collections.Specialized.StringCollection n1, string name)
        {
            if (name.Length > 5) { return _ = n1[Convert.ToInt32(name.Substring(7, 2)) - 1]; }
            else
            {
                if ((Convert.ToInt32(name.Substring(3, 2)) - 1) >= 0) { return _ = n1[Convert.ToInt32(name.Substring(3, 2)) - 1]; }
                else { return "- null -"; }
            }
        }

        /// <summary>
        /// Função para indentificar de acordo com a posição o 'name' Output
        /// </summary>
        /// <param name="n1"> StringCollection[] de output onde será procurado determinado 'name'</param>
        /// <param name="name"> String 'name' a ser procurado no StringCollection[]</param>
        /// <returns> Retorna string com a posição do name de saida</returns>
        public static string PositionIndexOUT(System.Collections.Specialized.StringCollection n1, string name)
        {
            if ((Convert.ToInt32(name.Substring(4, 2)) - 1) >= 0) { return _ = n1[Convert.ToInt32(name.Substring(4, 2)) - 1]; }
            else { return "- null -"; }
        }

        /// <summary>
        /// Função que insere o CHG0 na inicialização do sistema.
        /// </summary>
        /// <param name="systemType">string systemtype</param>
        /// <param name="vtPrimary">string name primário</param>
        /// <param name="vtSecondary">string name secundário</param>
        /// <param name="vtOutput">string name output</param>
        /// <param name="CHG0">string CHG0</param>
        public static void InitialInsertChg0(string systemType, System.Collections.Specialized.StringCollection vtPrimary, System.Collections.Specialized.StringCollection vtSecondary, System.Collections.Specialized.StringCollection vtOutput, string CHG0)
        {
            string chgo = "  CHG0    ";
            switch (systemType)
            {
                case "Change Over":
                    if (CHG0 == "IN 01  ") { vtPrimary[0] = vtSecondary[0] = vtOutput[0] = chgo; }
                    if (CHG0 == "IN 02  ") { vtPrimary[1] = vtSecondary[1] = vtOutput[1] = chgo; }
                    if (CHG0 == "IN 03  ") { vtPrimary[2] = vtSecondary[2] = vtOutput[2] = chgo; }
                    if (CHG0 == "IN 04  ") { vtPrimary[3] = vtSecondary[3] = vtOutput[3] = chgo; }
                    if (CHG0 == "IN 05  ") { vtPrimary[4] = vtSecondary[4] = vtOutput[4] = chgo; }
                    if (CHG0 == "IN 06  ") { vtPrimary[5] = vtSecondary[5] = vtOutput[5] = chgo; }
                    if (CHG0 == "IN 07  ") { vtPrimary[6] = vtSecondary[6] = vtOutput[6] = chgo; }
                    if (CHG0 == "IN 08  ") { vtPrimary[7] = vtSecondary[7] = vtOutput[7] = chgo; }
                    if (CHG0 == "IN 09  ") { vtPrimary[8] = vtSecondary[8] = vtOutput[8] = chgo; }
                    if (CHG0 == "IN 10  ") { vtPrimary[9] = vtSecondary[9] = vtOutput[9] = chgo; }
                    if (CHG0 == "IN 11  ") { vtPrimary[10] = vtSecondary[10] = vtOutput[10] = chgo; }
                    if (CHG0 == "IN 12  ") { vtPrimary[11] = vtSecondary[11] = vtOutput[11] = chgo; }
                    if (CHG0 == "IN 13  ") { vtPrimary[12] = vtSecondary[12] = vtOutput[12] = chgo; }
                    if (CHG0 == "IN 14  ") { vtPrimary[13] = vtSecondary[13] = vtOutput[13] = chgo; }
                    if (CHG0 == "IN 15  ") { vtPrimary[14] = vtSecondary[14] = vtOutput[14] = chgo; }
                    if (CHG0 == "IN 16  ") { vtPrimary[15] = vtSecondary[15] = vtOutput[15] = chgo; }
                    break;
                case "Matrix of Signals":
                    { }
                    break;
            }
        }
        //modelo do Indentifica: "[ Primary  :     IN 01 ] 'IN 01  ' [ AND ] [ Secondary:     IN 16 ] 'IN 16  ' ->[ OUT 13 ] 'OUT 13 '"
    }

    public class Functions
    {
        #region formatting
        /// <summary>
        /// Função para checar se durante a chamada de um Form, se algum  outro form está aberto.
        /// </summary>
        /// <returns>Retorna 1 se já tiver algum form aberto, retorna 0 se nenhum form estiver aberto</returns>
        public static int CheckFormOpen()
        {
            Form frC = Application.OpenForms["frmConfiguracoes"];
            Form frL = Application.OpenForms["frmLogs"];
            Form frS = Application.OpenForms["frmSobre"];
            int[] fr = new int[4];
            if (frC == null) { fr[0] = 0; } else { fr[0] = 1; }
            if (frL == null) { fr[1] = 0; } else { fr[1] = 1; }
            if (frS == null) { fr[2] = 0; } else { fr[2] = 1; }
            int sum = fr[0] + fr[1] + fr[2];
            if (sum == 0) { return 0; }
            if (fr[0] == 1) { frC.WindowState = FormWindowState.Normal; frC.TopMost = true; return 1; }
            if (fr[1] == 1) { frL.WindowState = FormWindowState.Normal; frL.TopMost = true; return 1; }
            if (fr[2] == 1) { frS.WindowState = FormWindowState.Normal; frS.TopMost = true; return 1; }
            return 1;
        }

        /// <summary>
        /// Função que retorna a aplicação para o Panel
        /// </summary>
        public static void ReturnPanel()
        {
            Form frC = Application.OpenForms["frmConfiguracoes"];
            Form frL = Application.OpenForms["frmLogs"];
            Form frS = Application.OpenForms["frmSobre"];
            Form frP = Application.OpenForms["frmPainel"];
            int[] fr = new int[4];
            if (frC == null) { fr[0] = 0; } else { fr[0] = 1; }
            if (frL == null) { fr[1] = 0; } else { fr[1] = 1; }
            if (frS == null) { fr[2] = 0; } else { fr[2] = 1; }
            int sum = fr[0] + fr[1] + fr[2];
            if (sum == 0) { frP.WindowState = FormWindowState.Normal; }
            if (fr[0] == 1) { frC.Close(); frP.WindowState = FormWindowState.Normal; frP.TopMost = true; }
            if (fr[1] == 1) { frL.Close(); frP.WindowState = FormWindowState.Normal; frP.TopMost = true; }
            if (fr[2] == 1) { frS.Close(); frP.WindowState = FormWindowState.Normal; frP.TopMost = true; }
        }

        /// <summary>
        /// Recebe 3 entradas booleanas e retorna uma string identificando qual o status das 3 entrada do tipo 'VVF'/'FVF' etc
        /// </summary>
        /// <param name="one"> Primeira entrada bool</param>
        /// <param name="two"> Segunda entrada bool</param>
        /// <param name="tree"> Segunda entrada bool </param>
        /// <returns> Retorna uma string identificando qual o status das 3 entrada do tipo 'VVF'/'FVF' etc</returns>
        public static string IdentifyTrueFalse(bool one, bool two, bool tree)
        {
            bool[] vetor = new bool[3];
            vetor[0] = one;
            vetor[1] = two;
            vetor[2] = tree;
            string str = "";
            if (vetor[0] == false) { str += "F"; }
            else { str += "V"; }
            if (vetor[1] == false) { str += "F"; }
            else { str += "V"; }
            if (vetor[2] == false) { str += "F"; }
            else { str += "V"; }
            return str;
        }

        /// <summary>
        /// Recebe string de entrada e retorna um Int
        /// </summary>
        /// <param name="input"> String de entrada para ser discretado/convertido em Int</param>
        /// <returns> Retorna Int discretando/convertendo a string de entrada</returns>
        public static int InputStringToInt(string input)
        {
            if (input.Length == 10) { return Convert.ToInt32(input.Substring(8, 1)); }
            else { return Convert.ToInt32(input.Substring(7, 1)); }
        }

        /// <summary>
        /// Recebe uma String de entrada e converte para Int (INPUT)
        /// </summary>
        /// <param name="input"> String de entrada a ser convertida para Int</param>
        /// <returns> Int convertido a saida da conversão da String de entrda</returns>
        public static int InputIntToString(string input)
        {
            switch (input)
            {
                case "IN 01": return 01;
                case "IN 02": return 2;
                case "IN 03": return 3;
                case "IN 04": return 4;
                case "IN 05": return 5;
                case "IN 06": return 6;
                case "IN 07": return 7;
                case "IN 08": return 8;
                case "IN 09": return 9;
                case "IN 10": return 10;
                case "IN 11": return 11;
                case "IN 12": return 12;
                case "IN 13": return 13;
                case "IN 14": return 14;
                case "IN 15": return 15;
                case "IN 16": return 16;
                default: return 0;
            }
        }

        /// <summary>
        /// Recebe uma String[] de entrada e converte para Int[] (INPUT)
        /// </summary>
        /// <param name="vetorstringentrada">  String[] de entrada a ser convertida para Int[]</param>
        /// <returns> Int[] convertido a saida da conversão da String[] de entrda</returns>
        public static int[] InputStringToInt(string[] input)
        {
            int[] saida = new int[17];
            for (int i = 0; i < input.Length; i++) { saida[i] = InputIntToString((input[i])); }
            return saida;
        }

        /// <summary>
        /// Recebe uma String de entrada e converte para Int (OUTPUT)
        /// </summary>
        /// <param name="entrada"> String de entrada a ser convertida para Int</param>
        /// <returns> Int convertido a saida da conversão da String de entrda</returns>
        public static int OutputStringToInt(string input)
        {
            switch (input)
            {
                case "OUT 01": return 0;
                case "OUT 02": return 1;
                case "OUT 03": return 2;
                case "OUT 04": return 3;
                case "OUT 05": return 4;
                case "OUT 06": return 5;
                case "OUT 07": return 6;
                case "OUT 08": return 7;
                case "OUT 09": return 8;
                case "OUT 10": return 9;
                case "OUT 11": return 10;
                case "OUT 12": return 11;
                case "OUT 13": return 12;
                case "OUT 14": return 13;
                case "OUT 15": return 14;
                case "OUT 16": return 15;
                default: return 16;
            }
        }

        /// <summary>
        /// Recebe uma String[] de entrada e converte para Int[] (OUTPUT)
        /// </summary>
        /// <param name="input"> String[] de entrada a ser convertida para Int[]</param>
        /// /// <returns> Int[] convertido a saida da conversão da String[] de entrda</returns>
        public static int[] OutputStringToInt(string[] input)
        {
            int[] output = new int[17];
            for (int i = 0; i < input.Length; i++) { output[i] = OutputStringToInt(input[i]); }
            return output;
        }

        /// <summary>
        ///  Recebe uma String de entrada e converte para Int (Implementado função NOT)
        /// </summary>
        /// <param name="input"> String de entrada a ser convertida para Int</param>
        /// <returns> Int convertido a saida da conversão da String de entrda</returns>
        public static int InputStringToInt_not(string input)
        {
            switch (input)
            {
                case "IN 01": return 0;
                case "IN 02": return 1;
                case "IN 03": return 2;
                case "IN 04": return 3;
                case "IN 05": return 4;
                case "IN 06": return 5;
                case "IN 07": return 6;
                case "IN 08": return 7;
                case "IN 09": return 8;
                case "IN 10": return 9;
                case "IN 11": return 10;
                case "IN 12": return 11;
                case "IN 13": return 12;
                case "IN 14": return 13;
                case "IN 15": return 14;
                case "IN 16": return 15;
                case "NOT-IN 01": return -100;
                case "NOT-IN 02": return -1;
                case "NOT-IN 03": return -2;
                case "NOT-IN 04": return -3;
                case "NOT-IN 05": return -4;
                case "NOT-IN 06": return -5;
                case "NOT-IN 07": return -6;
                case "NOT-IN 08": return -7;
                case "NOT-IN 09": return -8;
                case "NOT-IN 10": return -9;
                case "NOT-IN 11": return -10;
                case "NOT-IN 12": return -11;
                case "NOT-IN 13": return -12;
                case "NOT-IN 14": return -13;
                case "NOT-IN 15": return -14;
                case "NOT-IN 16": return -15;
                default: return 16;
            }
        }

        /// <summary>
        /// Retorna true se tiver algum item nulo ou vazio de  vetores
        /// </summary>
        /// <param name="input"> String[] de entrada poisção 1</param>
        /// <param name="input1"> String[] de entrada poisção 2</param>
        /// <param name="input2"> String[] de entrada poisção 3</param>
        /// <returns> Bool indicando se possui algum item nulo ou vazio em um dos 3 arrays de entrada</returns>
        public static bool NullOrempty(string[] input, string[] input1, string[] input2)
        {
            int qtd = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if ((string.IsNullOrEmpty(input[i]) || input[i].Trim().Length == 0) || (string.IsNullOrEmpty(input1[i]) || input1[i].Trim().Length == 0) || (string.IsNullOrEmpty(input2[i]) || input2[i].Trim().Length == 0))
                { qtd += 1; }
            }
            if (qtd == 0) { return false; }
            else { return true; }
        }

        /// <summary>
        /// Recebe uma string de entrada representando um index e retorna uma string com a 'IN' equivalente
        /// </summary>
        /// <param name="input"> String que representa um index de um vetor</param>
        /// <returns> Retorna a entrada 'IN' equivalente </returns>
        public static string WhatIn(string input)
        {
            switch (input)
            {
                case "0": return "IN 01";
                case "1": return "IN 02";
                case "2": return "IN 03";
                case "3": return "IN 04";
                case "4": return "IN 05";
                case "5": return "IN 06";
                case "6": return "IN 07";
                case "7": return "IN 08";
                case "8": return "IN 09";
                case "9": return "IN 10";
                case "10": return "IN 11";
                case "11": return "IN 12";
                case "12": return "IN 13";
                case "13": return "IN 14";
                case "14": return "IN 15";
                case "15": return "IN 16";
                case "16": return "IN 01";
                case "17": return "IN 02";
                case "18": return "IN 03";
                case "19": return "IN 04";
                case "20": return "IN 05";
                case "21": return "IN 06";
                case "22": return "IN 07";
                case "23": return "IN 08";
                case "24": return "IN 09";
                case "25": return "IN 10";
                case "26": return "IN 11";
                case "27": return "IN 12";
                case "28": return "IN 13";
                case "29": return "IN 14";
                case "30": return "IN 15";
                case "31": return "IN 16";
                default: return "IN 00";
            }
        }

        /// <summary>
        /// Recebe uma string de entrada representando um index e retorna uma string com a 'OUT' equivalente
        /// </summary>
        /// <param name="index"> String que representa um index de um vetor</param>
        /// <returns> Retorna a entrada 'OUT' equivalente </returns>
        public static string WhatOut(string index)
        {
            switch (index)
            {
                case "0": return "OUT 01";
                case "1": return "OUT 02";
                case "2": return "OUT 03";
                case "3": return "OUT 04";
                case "4": return "OUT 05";
                case "5": return "OUT 06";
                case "6": return "OUT 07";
                case "7": return "OUT 08";
                case "8": return "OUT 09";
                case "9": return "OUT 10";
                case "10": return "OUT 11";
                case "11": return "OUT 12";
                case "12": return "OUT 13";
                case "13": return "OUT 14";
                case "14": return "OUT 15";
                case "15": return "OUT 16";
                default: return "OUT 00";
            }
        }

        /// <summary>
        /// Função que recebe uma combinação e retorna um vetor com cada elemento da combinação
        /// </summary>
        /// <param name="input">string de entrada para ser extraida os elementos da combinação</param>
        /// <returns>string[] com os elementos da combinação</returns>
        public static string[] DiscreetCombination(string input)
        {
            /*
              00 device1
              01 input1
              02 name1
              03 logic
              04 device2
              05 input2
              06 name2
              07 out
              08 nameout
           */
            string[] s = new string[9];
            //quando infos do dev1 for nulo
            if (input.Substring(2, 1) == "-")
            {
                s[0] = "- null  -"; //device1
                s[1] = "- null  -"; //input1
                s[2] = "-null!-"; //name1
                s[3] = input.Substring(37, 3); //Logic
                s[4] = input.Substring(45, 9); //device2
                if (input.Substring(56, 4) == "NOT-")
                {
                    s[5] = input.Substring(56, 9); //input2
                }
                else
                {
                    s[5] = input.Substring(60, 5); //input2
                }
                s[6] = input.Substring(69, 7); //name2
                s[7] = input.Substring(82, 6); //out
                s[8] = input.Substring(92, 7); //nameout
            }
            else
            {
                s[0] = input.Substring(2, 9); //device1
                if (input.Substring(13, 4) == "NOT-")
                {
                    s[1] = input.Substring(13, 9);//input1
                }
                else
                {
                    s[1] = input.Substring(17, 5);//input1
                }
                s[2] = input.Substring(26, 7); //nome1
                s[3] = input.Substring(37, 3); //Logic
                if (input.Substring(45, 1) == "-")
                {
                    s[4] = "- null  -"; //device2
                    s[5] = "- null  -"; //input
                    s[6] = "-null!-"; //name2
                    s[7] = input.Substring(82, 6); //out
                    s[8] = input.Substring(92, 7); //nameout
                }
                else
                {
                    s[4] = input.Substring(45, 9);//device2
                    if (input.Substring(56, 4) == "NOT-")
                    {
                        s[5] = input.Substring(56, 9);//input2
                    }
                    else
                    {
                        s[5] = input.Substring(60, 5);//input2
                    }
                    s[6] = input.Substring(69, 7); //name2
                    s[7] = input.Substring(82, 6); //out
                    s[8] = input.Substring(92, 7); //nameout                                        
                }
            }
            return s;
        }

        /// <summary>
        /// Recebe os elementos da combinação e cria uma string com a combinação completa
        /// </summary>
        /// <param name="device1">string device 1</param>
        /// <param name="position1">string posição 1</param>
        /// <param name="name1">string name 1</param>
        /// <param name="logic">string logica operação</param>
        /// <param name="device2">string device 2</param>
        /// <param name="position2">string posição 2</param>
        /// <param name="name2">string name 2</param>
        /// <param name="output">string output</param>
        /// <param name="nameout">string name output</param>
        /// <returns>string com a combinação completa</returns>
        public static string EncryptCombination(string device1, string position1, string name1, string logic, string device2, string position2, string name2, string output, string nameout)
        {  
            _ = new string[] { device1, position1, name1, logic, device2, position2, name2, output, nameout };
            string j = "[ " + device1 + ": " + position1 + " ]" + " '" + name1 + "' " + "[ " + logic + " ] " + "[ " + device2 + ": " + position2 + " ]" + " '" + name2 + "' " + "->[ " + output + " ]" + " '" + nameout + "'";
            return j;
        }

        /// <summary>
        /// Recebe string de entrada e completa total de 7 caracteres
        /// </summary>
        /// <param name="input">string entrada para ser completada com total de 7 caracteres</param>
        /// <returns>string de entrada com total de 7 caracteres</returns>
        public static string Complete7Chars(string input)
        {
            int z;
            string s;
            switch (input.Length)
            {
                case 1: z = 6; s = input; break;
                case 2: z = 5; s = input; break;
                case 3: z = 4; s = input; break;
                case 4: z = 3; s = input; break;
                case 5: z = 2; s = input; break;
                case 6: z = 1; s = input; break;
                case 7: z = 0; s = input; break;
                default: s = input; z = 0; break;
            }
            for (int i = 0; i < z; i++) { s += " "; }
            return s;
        }

        #endregion

        #region Conversions
        /// <summary>
        /// fimção para corrigir problema do zero negativo
        /// </summary>
        /// <param name="input">entrada string representa a posição do input</param>
        /// <returns>int com numero da posição</returns>
        public static int ZeroNegative(int input)
        {
            if (input == -100) { return 0; }
            else { return input; }
        }

        /// <summary>
        /// Identifica se a operação lógica é AND 
        /// </summary>
        /// <param name="operation"Recebe string entrada que representa a opração a se identificada></param>
        /// <returns> Retorna bool caso a sring de entrada seja 'AND'</returns>
        public static bool IsAnd(string operation)
        {
            if (operation.ToUpper() == "AND") { return true; }
            else { return false; }
        }

        /// <summary>
        /// Identifica se a operação lógica é  OR 
        /// </summary>
        /// <param name="operation"> Recebe string entrada que representa a opração a se identificada</param>
        /// <returns> Retorna bool caso a sring de entrada seja ' OR'</returns>
        public static bool IsOr(string operation)
        {
            if (operation.ToUpper() == " OR") { return true; }
            else { return false; }
        }

        /// <summary>
        /// Identifica se a operação lógica é NOT 
        /// </summary>
        /// <param name="operation"> Recebe string entrada que representa a opração a se identificada</param>
        /// <returns> Retorna bool caso a sring de entrada seja 'NOT'</returns>
        public static bool IsNot(string operation)
        {
            if (operation.ToUpper() == "NOT")
            { return true; }
            else { return false; }
        }

        /// <summary>
        /// Realiza a operação lógica AND 
        /// </summary>
        /// <param name="n1"> Bool que representa o primeiro numero para realiazr a operação AND</param>
        /// <param name="n2"> Bool que representa o segundo numero para realiazr a operação AND</param>
        /// <returns> Retorna Bool com o resultado da operação AND entre 'n1 e 'n2'</returns>
        public static bool OperationAND(bool n1, bool n2)
        {
            bool resp = false;
            if (n1 == false && n2 == false) { resp = false; }
            if (n1 == false && n2 == true) { resp = false; }
            if (n1 == true && n2 == false) { resp = false; }
            if (n1 == true && n2 == true) { resp = true; }
            return resp;
        }

        /// <summary>
        /// Realiza a operação lógica OR 
        /// </summary>
        /// <param name="n1"> Bool que representa o primeiro numero para realiazr a operação OR</param>
        /// <param name="n2"> Bool que representa o segundo numero para realiazr a operação OR</param>
        /// <returns> Retorna Bool com o resultado da operação OR entre 'n1 e 'n2'</returns>
        public static bool OperationOR(bool n1, bool n2)
        {
            bool resp = true;
            if (n1 == false && n2 == false) { resp = false; }
            if (n1 == false && n2 == true) { resp = true; }
            if (n1 == true && n2 == false) { resp = true; }
            if (n1 == true && n2 == true) { resp = true; }
            return resp;
        }

        /// <summary>
        /// Realiza a operação lógica NOT 
        /// </summary>
        /// <param name="n"> Bool para realiazr a operação NOT</param>
        /// <returns> Retorna Bool com o resultado da operação NOT com a entrada 'n1'</returns>
        public static bool OperationNOT(bool n)
        {
            bool resp;
            if (n == false) { resp = true; }
            else { resp = false; }
            return resp;
        }

        /// <summary>
        ///  Função para escolher o formato da Data dpo sistema
        /// </summary>
        /// <returns>Data de acordo com o formato que está configurado no computador</returns>
        public static string GetDateSystem()
        {
            string Data;
            string Dc = DateTime.Now.ToLongDateString();
            if (Dc.Substring(0, 1) == "0" || Dc.Substring(0, 1) == "1" || Dc.Substring(0, 1) == "2" || Dc.Substring(0, 1) == "3" || Dc.Substring(0, 1) == "4" || Dc.Substring(0, 1) == "5" || Dc.Substring(0, 1) == "6" || Dc.Substring(0, 1) == "7" || Dc.Substring(0, 1) == "8" || Dc.Substring(0, 1) == "9" || Dc.Substring(0, 1) == "0")
            { Data = DateTime.Now.ToShortDateString(); }
            else { Data = DateTime.Now.ToLongDateString(); }
            return Data;
        }

        /// <summary>
        /// Função que recebe uma string de entrada e formata para byte.
        /// </summary>
        /// <param name="input"> String de entrada a ser formatada para byte</param>
        /// <returns> Retorna a string de saída formatada para byte com 8 digitos</returns>
        public static string FormatByte(string input)
        {
            byte[] Data = new byte[16];
            Data[0] = Convert.ToByte(input);
            string bin = DecimalToBinary(Data[0].ToString());
            string bits = Functions.Complete8Chars(bin);
            string bina = InvertString(bits);
            _ = BinaryToDecimal(bits).ToString();
            return bina;
        }

        /// <summary>
        /// Função que recebe 1 string e converte para um vetor booleano.
        /// </summary>
        /// <param name="input"> String 1 de entrada a ser transformada em um vetor booleano</param>
        /// <returns> Retorna um booleano[] que representa as strings de entrada</returns>
        public static bool[] VetorBoolean(string input)
        {
            bool[] vetor = new bool[16];
            vetor[0] = Convert.ToBoolean(Convert.ToDecimal(input.Substring(0, 1)));
            vetor[1] = Convert.ToBoolean(Convert.ToDecimal(input.Substring(1, 1)));
            vetor[2] = Convert.ToBoolean(Convert.ToDecimal(input.Substring(2, 1)));
            vetor[3] = Convert.ToBoolean(Convert.ToDecimal(input.Substring(3, 1)));
            vetor[4] = Convert.ToBoolean(Convert.ToDecimal(input.Substring(4, 1)));
            vetor[5] = Convert.ToBoolean(Convert.ToDecimal(input.Substring(5, 1)));
            vetor[6] = Convert.ToBoolean(Convert.ToDecimal(input.Substring(6, 1)));
            vetor[7] = Convert.ToBoolean(Convert.ToDecimal(input.Substring(7, 1)));
            return vetor;
        }

        /// <summary>
        /// Função que recebe string e completa para 8 digitos de um byte
        /// </summary>
        /// <param name="input"> String de entrada</param>
        /// <returns> Retorna uma string de saida com 8 digitos</returns>
        /// 
        public static string Complete8Chars(string input)
        {
            int tamanho = input.Length;
            string saida = input;
            if (tamanho == 8 || tamanho == 0) { saida = input; }
            if (tamanho == 7) { saida = "0" + input; }
            if (tamanho == 6) { saida = "00" + input; }
            if (tamanho == 5) { saida = "000" + input; }
            if (tamanho == 4) { saida = "0000" + input; }
            if (tamanho == 3) { saida = "00000" + input; }
            if (tamanho == 2) { saida = "000000" + input; }
            if (tamanho == 1) { saida = "0000000" + input; }
            return saida;
        }

        /// <summary>
        /// Função que converte um decimal para binário
        /// </summary>
        /// <param name="input"> String de entrada para ser convertida para binário</param>
        /// <returns> Retorna uma string representando o valor de entrada em binário</returns>
        public static string DecimalToBinary(string input)
        {
            string valor = "";
            int dividendo = Convert.ToInt32(input);
            if (dividendo == 0 || dividendo == 1)
            { return Convert.ToString(dividendo); }
            else
            {
                while (dividendo > 0)
                { valor += Convert.ToString(dividendo % 2); dividendo /= 2; }
                return InvertString(valor);
            }
        }

        /// <summary>
        /// Função que inverte os caracteres de uma string
        /// </summary>
        /// <param name="input"> String de entrada para ser invertida</param>
        /// <returns> Retorna invertid a string de entrada</returns>
        public static string InvertString(string input)
        {
            int tamanho = input.Length;
            char[] caracteres = new char[tamanho];
            for (int i = 0; i < tamanho; i++) { caracteres[i] = input[tamanho - 1 - i]; }
            return new string(caracteres);
        }

        /// <summary>
        /// Função converte um binário para decimal.
        /// </summary>
        /// <param name="input"> String de entrada representando um valor em binário a ser convertido para decimal</param>
        /// <returns >Retorna um int (Decimal) com o valor da entrada em binário </returns>
        public static int BinaryToDecimal(string input)
        {
            int expoente = 0;
            int numero;
            int soma = 0;
            string numeroInvertido = InvertString(input);
            for (int i = 0; i < numeroInvertido.Length; i++)
            {
                //pega dígito por dígito do número digitado
                numero = Convert.ToInt32(numeroInvertido.Substring(i, 1));
                //multiplica o dígito por 2 elevado ao expoente, e armazena o resultado em soma
                soma += numero * (int)Math.Pow(2, expoente);
                // incrementa o expoente
                expoente++;
            }
            return soma;
        }

        public static bool StringToBoolean(string input)
        {
            if (input == "True")
            { return true; }
            else
            { return false; }
        }
       
        #endregion
    }

    public class Records
    {
        /// <summary>
        /// Função indicando/Registrando status das entradas 1-16
        /// </summary>
        /// <param name="inputs"> Int de entrada representando as entradas do Secondary 1-16</param>
        public static string StatusIn1a16ChgT(bool[] read)
        {
            string[] inputs = FormatVetorStatus(read);
            string historico;
            historico = "<" + DateTime.Now.ToString() + " - Secondary  " + InOnlyOn(inputs);
            return historico;
        }

        /// <summary>
        /// Função indicando/Registrando status das entradas 1-16
        /// </summary>
        /// <param name="inputs"> Int de entrada representando as entradas do Secondary 1-16</param>
        public static string StatusIn1a16ChgR(bool[] read)
        {
            string[] inputs = FormatVetorStatus(read);
            string historico;
            historico = "<" + DateTime.Now.ToString() + " - Primary    " + InOnlyOn(inputs);
            return historico;
        }

        /// <summary>
        /// Função para registar apenas como a entrada estiver ativa.
        /// </summary>
        /// <param name="vetorstringentrada"> String[] entrada para verificar quais estão ativas</param>
        /// <returns> String representando as entradas acionadas</returns>
        public static string InOnlyOn(string[] vetorstringentrada)
        {
            string Saida = "";
            for (int i = 0; i < vetorstringentrada.Length; i++)
            {
                if (vetorstringentrada[i] == "ON; ")
                {
                    string s = "0";
                    if ((i + 1).ToString().Length > 1) { s = (i + 1).ToString(); }
                    else { s += (i + 1).ToString(); }
                    Saida += " " + "IN" + " " + (s) + ">";
                }
            }
            return Saida;
        }

        /// <summary>
        /// Função para registar apenas como a entrada estiver ativa.
        /// </summary>
        /// <param name="vetorstringentrada"> String[] de entradas para verificar quais estão acionadas, usada nos LOGS</param>
        /// <returns> String representa quais entradas acionadas</returns>
        public static string InOnlyOnLog(string[] vetorstringentrada)
        {
            string Saida = "";
            for (int i = 0; i < vetorstringentrada.Length; i++)
            {
                if (vetorstringentrada[i] == "ON; ")
                {
                    string s = "0";
                    if ((i + 1).ToString().Length > 1) { s = (i + 1).ToString(); }
                    else { s += (i + 1).ToString(); }
                    Saida += " " + "IN" + (s) + "> ";
                }
            }
            return Saida;
        }

        /// <summary>
        /// Função para registrar nos logs as saidas dos slots
        /// </summary>
        /// <param name="read">entrada booleana representando a resposta das combinações</param>
        /// <returns>retorna string com as saidas acionadas</returns>
        public static string SlotsLogs(bool[] read)
        {
            string Saida = "";
            for (int i = 0; i < read.Length; i++)
            {
                if (read[i] == true)
                {
                    string s = "0";
                    if ((i + 1).ToString().Length > 1) { s = (i + 1).ToString(); }
                    else { s += (i + 1).ToString(); }
                    Saida += "<" + DateTime.Now.ToString() + " - " + " OUT" + " " + (s) + "> ";
                }
            }
            return Saida;
        }

        /// <summary>
        /// Função para registar apenas como a saida estiver ativa
        /// </summary>
        /// <param name="Entrada"> String entrada para verificar quais saidas acionadas, usado em LOGS</param>
        /// <returns> String representa quais saidas acionadas</returns>
        public static string OutOnlyOnLog(string[] input)
        {
            string Saida = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == "ON; ")
                {
                    string s = "0";
                    if ((i + 1).ToString().Length > 1) { s = (i + 1).ToString(); }
                    else { s += (i + 1).ToString(); }
                    Saida += " " + "OUT" + (s) + "> ";
                }
            }
            return Saida;
        }

        /// <summary>
        /// Converte vetor booleano em um vetor String (On, Off)
        /// </summary>
        /// <param name="inputs"> Bool[] entrada a ser convertido em String[] ON/OFF</param>
        /// <returns> String[] ON/OFF convertido da entrada Bool[]</returns>
        public static string[] FormatVetorStatus(params bool[] inputs)
        {
            string[] vs = new string[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i] == true) { vs[i] = "ON; "; }
                else { vs[i] = "OFF; "; }
            }
            return vs;
        }

        /// <summary>
        /// Retorna true caso algum elemento do vetor for true
        /// </summary>
        /// <param name="inputs"> Bool[] entrada para ser verificado se alguma elemento é TRUE</param>
        /// <returns> Bool representa se tem algum elemento TRUE no Bool[] de entrada</returns>
        public static bool TemTrue(params bool[] inputs)
        {
            bool vs;
            if (inputs.Contains(true)) { vs = true; }
            else { vs = false; }
            return vs;
        }
    }

    public class Logger
    {
        /// <summary>
        /// Configura um appender apenas com o path como paramentro no construtor
        /// </summary>
        /// <param name="logFile"> String de entrada com path para arquivo de log</param>
        public static void ConfigureFileAppender(string logFile)
        {
            IAppender fileAppender = GetFileAppender(logFile);
            BasicConfigurator.Configure(fileAppender);
            ((Hierarchy)LogManager.GetRepository()).Root.Level = Level.Debug;
        }

        /// <summary>
        /// Configura um Appender com path, max size, dados a serem gravados como paramentros de construtor
        /// </summary>
        /// <param name="logFile"> String de entrada com path para arquivo de log</param>
        /// <param name="maxSize"> Int representado o tamanho máximo de arquivo</param>
        /// <param name="dataLog"> String[] com as opções de escolha de seleção dos logs</param>
        public static void ConfigureFileAppender(string logFile, int maxSize, string[] dataLog)
        {
            IAppender fileAppender = GetFileAppenderSize(logFile, maxSize, dataLog);
            BasicConfigurator.Configure(fileAppender);
            ((Hierarchy)LogManager.GetRepository()).Root.Level = Level.Debug;
        }

        /// <summary>
        /// Configura um Appender com path, max size, dados a serem gravados e se está habilitado a criação de logs do sistma como paramentros de construtor
        /// </summary>
        /// <param name="logFile"> String de entrada com path para arquivo de log</param>
        /// <param name="maxSize"> Int representado o tamanho máximo de arquivo</param>
        /// <param name="dataLog"> String[] com as opções de escolha de seleção dos logs</param>
        /// <param name="enable"> Bool indicando se a gravação do Logs está habilitada no sistema</param>
        public static void ConfigureFileAppender(string logFile, int maxSize, string[] dataLog, bool enable)
        {
            if (enable == true)
            {
                IAppender fileAppender = GetFileAppenderSize(logFile, maxSize, dataLog);
                BasicConfigurator.Configure(fileAppender);
                ((Hierarchy)LogManager.GetRepository()).Root.Level = Level.Debug;
            }
        }

        /// <summary>
        /// Configura um appender apenas com o path como paramentro no construtor
        /// </summary>
        /// <param name="logFile"> String de entrada com path para arquivo de log</param>
        /// <returns> IAppender configurado com path de entrada</returns>
        private static IAppender GetFileAppender(string logFile)
        {
            PatternLayout layout = new PatternLayout("%date; %username; [%thread]; %-5level; %method; Message: %message; %newline");
            layout.ActivateOptions();
            RollingFileAppender appender = new RollingFileAppender
            {
                MaximumFileSize = "100MB",
                File = logFile,
                Encoding = Encoding.UTF8,
                Threshold = Level.All,
                Layout = layout
            };
            appender.ActivateOptions();
            return appender;
        }

        /// <summary>
        /// Configura um Appender com path, max size, dadosa serem gravados como paramentros de construtor
        /// </summary>
        /// <param name="logFile"> String de entrada com path para arquivo de log</param>
        /// <param name="maxSize"> Int representado o tamanho máximo de arquivo</param>
        /// <param name="dataLog"> String[] com as opções de escolha de seleção dos logs</param>
        /// <returns> IAppender configurado com path de entrada, tamanho maximo e dados a serem gravados do sistema</returns>
        private static IAppender GetFileAppenderSize(string logFile, int maxSize, string[] dataLog)
        {
            // var layout = new PatternLayout("%date; %username; [%thread]; %-5level; %method; Message: %message; %newline");
            PatternLayout layout = new PatternLayout(DecodeData(dataLog));
            layout.ActivateOptions();
            RollingFileAppender appender = new RollingFileAppender
            {
                MaximumFileSize = Convert.ToString(maxSize) + "MB",
                File = logFile,
                Encoding = Encoding.UTF8,
                Threshold = Level.All,
                Layout = layout

            };
            appender.ActivateOptions();
            return appender;
        }

        /// <summary>
        /// Recebe uma vetor string de entrada e converte para uma string contendo os dados a serem gravados
        /// </summary>
        /// <param name="ray"> String[] de entrada indicando os dados a serem salvos do sistema</param>
        /// <returns> String com os dados a serem gravados do sistema</returns>
        public static string DecodeData(string[] ray)
        {
            //1;2;3;4;5;6;7;8;
            string saida = "%date" + "; ";
            if (ray[0] == "TRUE") { saida = saida + "%username" + "; "; }
            if (ray[1] == "TRUE") { saida = saida + "%-5level" + "; "; }
            if (ray[2] == "TRUE") { saida = saida + " %method" + "; "; }
            if (ray[3] == "TRUE") { saida = saida + "%message" + "; "; }
            if (ray[4] == "TRUE") { saida = saida + "[%thread]" + "; "; }
            if (ray[5] == "TRUE") { saida = saida + "%line" + "; "; }
            if (ray[6] == "TRUE") { saida = saida + "%identity" + "; "; }
            if (ray[7] == "TRUE") { saida = saida + "%location" + "; "; }
            saida += "%newline";
            return saida;
        }
    }
}