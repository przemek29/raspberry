
using System;
using System.Diagnostics;
using System.Threading;

namespace program
{

public class adxl345
{
	// dostęp do lokalizacji programu i2cget
    private string i2cgetExe = "/usr/sbin/i2cget"; 
	// '-y' wyłącza tryb interatywny - używanie tego znacznika powoduje wykonywanie operacji bezpośrednio bez potwierdzenia 
	// '1' oznacza nr szyny danych dla RPi 512Mb 
	// 0x53 adres urządzenia adxl345
	// 0x32 do 0x37 rejestry odpowiedzialne za pakiet danych X,Y,Z
    private string i2cgetDataX0 = "-y 1 0x53 0x32"; 
    private string i2cgetDataX1 = "-y 1 0x53 0x33"; 
    private string i2cgetDataY0 = "-y 1 0x53 0x34"; 
	private string i2cgetDataY1 = "-y 1 0x53 0x35"; 
	private string i2cgetDataZ0 = "-y 1 0x53 0x36"; 
	private string i2cgetDataZ1 = "-y 1 0x53 0x37"; 
	
	private string hexString = "";
	private Process p;

    public adxl345()
    {
        p = new Process();
    }

    public double tempX
    {
		get { return readX0Data(); }
    }
    

    private int readX0Data()
    {
		
        // Don't raise event when process exits
        p.EnableRaisingEvents = false;
        // We're using an executable not document, so UseShellExecute false
        p.StartInfo.UseShellExecute = false;
        // Redirect StandardError
        p.StartInfo.RedirectStandardError = true;
        // Redirect StandardOutput so we can capture it
        p.StartInfo.RedirectStandardOutput = true;
        // i2cgetExe has full path to executable
        // Need full path because UseShellExecute is false

        p.StartInfo.FileName = i2cgetExe;
        // Pass arguments as a single string
        p.StartInfo.Arguments = i2cgetDataX0 ;
        // Now run i2cget & wait for it to finish
        p.Start();
        p.WaitForExit();
        // Data returned in format 0x00
        
        string data = p.StandardOutput.ReadToEnd();
        // Get LSB & parse as integer
			hexString = data.Substring(2,2);
        int lsb = Int32.Parse(hexString, 
		                      System.Globalization.NumberStyles.AllowHexSpecifier);
        
        
        // Shift bits as indicated in TMP102 docs & return
        //return (((msb << 8) | lsb) >> 4);
		return lsb;

	}

    public static void Main ()
		{
			adxl345 a = new adxl345 ();
			while (true) {
				
				Console.WriteLine (a.tempX);

				Thread.Sleep (1000);
			}
		}   
	
}
}


