using System;
using System.Windows.Forms;
using Sealevel;


namespace CHOV
{
    public class Sea_Lvl : SeaMAX
    {
            //Declarando variavel Handler Sealevel.
            public static SeaMAX SeaMAX_DeviceHandler = new SeaMAX();

            /// <summary>
            /// OK-Função que abre e estabelece conexão com dispositivo Sealevel.
            /// </summary>
            /// <param name="id"> ip do dispositivo a iniciar a conexão</param>
            /// <returns>mensagem resposta da função</returns>
            public static string AbreConexao(string id)
            {
                string res = "";
                int ret = SeaMAX_DeviceHandler.SM_Open(id);
                if (ret >= 0) { res = "Success in opening the connection! " + "cod: " + ret; return res; }
                else
                {
                    switch (ret)
                    {
                        case -1: res = "Parameter 'connection' is null!" + "cod: " + ret; return res;
                        case -2: res = "Could not determine connetion type" + "cod: " + ret; return res;
                        case -3: res = "Invalid connection string." + "cod: " + ret; return res;
                        case -10: res = "Serial: Invalid or unavailable serial connection." + "cod: " + ret; return res;
                        case -11: res = "Serial: Unable to acquire a valid mutex." + "cod: " + ret; return res;
                        case -12: res = "Serial: Unable to set serial timeouts." + "cod: " + ret; return res;
                        case -13: res = "Serial: Unable to set serial parameters (e.g. baudrate, parity, etc.)." + "cod: " + ret; return res;
                        case -14: res = "Serial: Invalid serial name para meter." + "cod: " + ret; return res;
                        case -20: res = "Ethernet: Could not resolve host address." + "cod: " + ret; return res;
                        case -21: res = "Ethernet: Host refused or unavailable." + "cod: " + ret; return res;
                        case -22: res = "Ethernet: Could not acqure free socket." + "cod: " + ret; return res;
                        case -23: res = "Ethernet: Could not acquire a valid mutex refused or unavaiable." + "cod: " + ret; return res;
                        case -30: res = "SeaDAC Lite: Invalid or unavailable port." + "cod: " + ret; return res;
                        case -31: res = "SeaDAC Lite: Unable to acquire a mutex handle." + "cod: " + ret; return res;
                        case -32: res = "SeaDAC Lite: Invalid device number (should be zero or greater)." + "cod: " + ret; return res;
                        case -33: res = "SeaDAC Lite: Could not read Vendor ID." + "cod: " + ret; return res;
                        case -34: res = "SeaDAC Lite: Could not read Product ID." + "cod: " + ret; return res;
                        case -40: res = "Could not read USB device product or vendor ID." + "cod: " + ret; return res;
                        case -41: res = "Non-SeaLevel USB device." + "cod: " + ret; return res;
                        case -42: res = "SeaMAX doesnot support this Sealevel USB device." + "cod: " + ret; return res;
                        default: return res;
                    }
                }
            }

            /// <summary>
            /// OK-Função que encerra a conexão com o dispositivo Sealevel
            /// </summary>
            /// <param name="SeaMAX_DeviceHandler">Dispositivo Sealevel de entrada, devidamente instanciado</param>
            /// <returns>mensagem resposta da função</returns>
            public static string FechaConexao()
            {
                string resultado = "";
                if (SeaMAX_DeviceHandler.IsSeaMAXOpen)
                {
                    if (SeaMAX_DeviceHandler.SM_Close() >= 0) { resultado = "Successful closure and cleanup."; }
                    else { resultado = "Invalid SeaMAX handle."; }
                }
                return resultado;
            }

            /// <summary>
            /// OK-MELHORAR! Função que inicializa a ethernet do módulo Sealevel 
            /// </summary>
            /// <returns>mensagem resposta da função</returns>
            public static string[] Inicial()
            {
                _ = new bool[32];
                string[] p = new string[6];
                string p1 = "*";
                string p2 = "*";
                string p3 = "*";
                string p5 = "*";
                string p6 = "*";
                try
                {
                    //If the Ethernet API is not initialized (it won't be at this point) then initialize it
                    if (!SeaMAX_DeviceHandler.IsEthernetInitialized) { int sts = SeaMAX_DeviceHandler.SME_Initialize(); p1 = (Convert.ToString(sts) + " " + "(initialized;se = 0, entao SUCCESS)"); }
                    //Do the initial search for modules on the network
                    int ModuleCount = SeaMAX_DeviceHandler.SME_SearchForModules();
                    if (ModuleCount == 0) { p2 = ("(search for modules on the network)" + " " + "No devices found."); }
                    else if (ModuleCount < 0) { p3 = ("Error " + ModuleCount.ToString() + " searching for devices."); }
                    string p4 = (ModuleCount.ToString() + " devices found.");
                    //Select the first device found
                    int errno = SeaMAX_DeviceHandler.SME_FirstModule();
                    if (errno < 0) { p5 = ("Error selecting first device." + errno); }
                    ModuleCount = 1;
                    //Iterate through the devices printing them to the console
                    for (int i = 0; i < ModuleCount; i++)
                    {
                        //Print the device to the console
                        p6 = DisplayDevice(SeaMAX_DeviceHandler);
                        //Select the next module
                        errno = SeaMAX_DeviceHandler.SME_NextModule();
                        if (errno == -3)
                        {
                            // listBox1.Items.Add("errno = -3; Fim dos módulos descobertos ");
                            //break;
                        }
                    }
                    p[0] = p1; p[1] = p2; p[2] = p3; p[3] = p4; p[4] = p5; p[5] = p6;
                    return p;
                    //listBox1.Items.Add("End of discovered modules.");
                }
                catch (Exception) { return p; }
                finally { }
            }

            /// <summary>
            /// OK-MELHORAR!Função que obtem e exibe as infomações do dispositivo
            /// </summary>
            /// <param name="SeaMAX_DeviceHandler"></param>
            /// <returns> string com a mensagem obtida como resposta</returns>
            public static string DisplayDevice(SeaMAX SeaMAX_DeviceHandler)
            {
                //Ping the device to make sure it is still available
                int errno = SeaMAX_DeviceHandler.SME_Ping();
                //Save the IP Address
                string ip = "";
                string netmask = "";
                string gateway = "";
                SeaMAX_DeviceHandler.SME_GetNetworkConfig(ref ip, ref netmask, ref gateway);
                // listBox1.Items.Add("errno = " + errno);
                if (errno < 1)
                {
                    //   listBox1.Items.Add("Device at " + ip + " failed to respond.");
                }
                string name = "";
                errno = SeaMAX_DeviceHandler.SME_GetName(ref name);
                if (errno < 0)
                {
                    // listBox1.Items.Add("Could not retrieve name of device at " + ip);
                }
                string pp = "Device at " + ip + " is named " + name + ".";
                return pp;
            }

            /// <summary>
            /// OK-Função que realiza a leitura das entradas e retorna uma string com valor em decimal das entradas acionadas. Retorna 'X' caso dê erro na leitura
            /// </summary>
            /// <param name="select"> dispositivo a ser lido as entradas</param>
            /// <returns>string com entradas acionadas</returns>
            public static string LeituraEntradas(string select)
            {
                //Leitura entradas
                byte[] Data1 = new byte[16];
                string saida = "";
                //Resposta do comando.
                int read = SeaMAX_DeviceHandler.SM_ReadDigitalInputs(0, 16, Data1);
                if (read >= 0)
                {
                    if (select == "Secondary") { saida = Data1[1].ToString(); }
                    else { saida = Data1[0].ToString(); }
                    return saida;
                }
                else
                {
                    switch (read)
                    {
                        case -1: MessageBox.Show("Invalid SeaMAX handle." + "cod: " + read, "Error"); return saida;
                        case -2: MessageBox.Show("Modbus: Connection is not established. Check the provided Connection object state." + "cod: " + read, "Error"); return saida;
                        case -3: MessageBox.Show("Modbus: Read error waiting for response. Unknown Modbus exception." + "cod: " + read, "Error"); return saida;
                        case -4: MessageBox.Show("Modbus: Illegal Modbus Function (Modbus Exception 0x01)." + "cod: " + read, "Error"); return saida;
                        case -5: MessageBox.Show("Modbus: Illegal Data Address (Modbus Exception 0x02)." + "cod: " + read, "Error"); return saida;
                        case -6: MessageBox.Show("Modbus: Illegal Data Value(Modbus Exception 0x03)." + "cod: " + read, "Error"); return saida;
                        case -7: MessageBox.Show("Modbus CRC was invalid. Possible communications problem." + "cod: " + read, "Error"); return saida;
                        case -20: MessageBox.Show("SeaDAC Lite: Invalid model number." + "cod: " + read, "Error"); return saida;
                        case -21: MessageBox.Show("SeaDAC Lite: Invalid addressing." + "cod: " + read, "Error"); return saida;
                        case -22: MessageBox.Show("SeaDAC Lite: Error reading the device." + "cod: " + read, "Error"); return saida;
                        default: MessageBox.Show("SeaDAC Lite: Error reading the device." + "cod: " + read, "Error"); return saida;
                    }
                }
            }

            /// <summary>
            /// OK-Função que ativa as saidas correspondentes.
            /// </summary>
            /// <param name="INativa">string de entrada representado o valor das entradas ativas em decimal</param>
            /// <returns>string com entradas acionadas</returns>
            public static string EscreveSaidas(string INativa)
            {
                string resp = "";
                //Escreve saidas
                byte[] Data = new byte[16];
                Data[0] = Convert.ToByte(INativa);
                //Resposta do comando.
                int ret1 = SeaMAX_DeviceHandler.SM_WriteDigitalOutputs(0, 16, Data);
                if (ret1 >= 0) { resp = "Number of bytes successfully written." + "cod: " + ret1; return resp; }
                else
                {
                    switch (ret1)
                    {
                        case -1: resp = "Invalid SeaMax handle." + "cod: " + ret1; MessageBox.Show(resp, "Error"); return resp;
                        case -2: resp = "Modbus: Connection is not established. Check the provided Connection object state." + "cod: " + ret1; return resp;
                        case -3: resp = "Modbus:Read error waiting for response. Unknown Modbus exception." + "cod: " + ret1; MessageBox.Show(resp, "Error"); return resp;
                        case -4: resp = "Modbus: Illegal Modbus Function (Modbus Exception 0x01)." + "cod: " + ret1; MessageBox.Show(resp, "Error"); return resp;
                        case -5: resp = "Modbus: Illegal Data Address (Modbus Exception 0x02)." + "cod: " + ret1; MessageBox.Show(resp, "Error"); return resp;
                        case -6: resp = "Modbus: Illegal Data Value (Modbus Exception 0x03)." + "cod: " + ret1; MessageBox.Show(resp, "Error"); return resp;
                        case -7: resp = "Modbus RC was invalid. Possible communications problem." + "cod: " + ret1; MessageBox.Show(resp, "Error"); return resp;
                        case -20: resp = "SEADAC Lite: Invalid model number." + "cod: " + ret1; MessageBox.Show(resp, "Error"); return resp;
                        case -21: resp = "SEADAC Lite: Invalid addressing." + "cod: " + ret1; MessageBox.Show(resp, "Error"); return resp;
                        case -22: resp = "SEADAC Lite: Error writting to the device." + "cod: " + ret1; MessageBox.Show(resp, "Error"); return resp;
                        default: return resp;
                    }
                }
            }

    }
}
