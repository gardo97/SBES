﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SymetricAlgorithmAES
{
    public class SecretKey
    {
		/// <summary>
		/// Generate a symmetric key for the specific symmetric algorithm. IV is generated automatically.
		/// </summary>
		/// <param name="algorithmType"> type of symmetric algorith the key is generated for </param>
		/// <returns> string value representing a symmetric key </returns>
		public static string GenerateKey()
		{
			SymmetricAlgorithm symmAlgorithm = null;
			symmAlgorithm = AesCryptoServiceProvider.Create();
			
			return symmAlgorithm == null ? String.Empty : ASCIIEncoding.ASCII.GetString(symmAlgorithm.Key);
		}

		/// <summary>
		/// Store a secret key as string value in a specified file.
		/// </summary>
		/// <param name="secretKey"> a symmetric key value </param>
		/// <param name="outFile"> file location to store a secret key </param>
		public static void StoreKey(string secretKey, string outFile)
		{
			FileStream fOutput = new FileStream(outFile, FileMode.OpenOrCreate, FileAccess.Write);
			byte[] buffer = Encoding.ASCII.GetBytes(secretKey);

			try
			{
				fOutput.Write(buffer, 0, buffer.Length);
			}
			catch (Exception e)
			{
				Console.WriteLine("SecretKeys.StoreKey:: ERROR {0}", e.Message);
			}
			finally
			{
				fOutput.Close();
			}
		}

		

		

		/// <summary>
		/// Load a symmetric key value from a file
		/// </summary>
		/// <param name="inFile"> file location of a secret key </param>
		/// <returns> a secret key value </returns>
		public static string LoadKey(string inFile)
		{
			FileStream fInput = new FileStream(inFile, FileMode.Open, FileAccess.Read);
			byte[] buffer = new byte[(int)fInput.Length];

			try
			{
				fInput.Read(buffer, 0, (int)fInput.Length);
			}
			catch (Exception e)
			{
				Console.WriteLine("SecretKeys.LoadKey:: ERROR {0}", e.Message);
			}
			finally
			{
				fInput.Close();
			}

			return ASCIIEncoding.ASCII.GetString(buffer);
		}

	}
}