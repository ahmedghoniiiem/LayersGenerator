﻿using Generator.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace Generator
{
	internal class Program
	{

		private static void Main(string[] args)
		{
			FileHelper fileHelper = new FileHelper();
			PluralizationHelper pluralizationHelper = new PluralizationHelper();
			
			//get Current Domain Path
			string path = AppDomain.CurrentDomain.BaseDirectory;
		
			//Read XML template String
			string fileContents = fileHelper.ReadFile(path, "Model.XML");

			//Load XML template model 
			XMLHelper xmlHelper = new XMLHelper();
			Model.Model model = xmlHelper.DeserializeText<Model.Model>(fileContents);
			Console.WriteLine("Please Enter Model Name");
			string modelName = Console.ReadLine();
			string instanceModel = pluralizationHelper.GetInstanceName(modelName);
			string BaseModelName = "BaseModel";
			string PKType = "int";

			//Loop for each template item
			foreach (Item item in model.Items)
			{
				string contents = item.Class;
				//replace items
				contents = contents.Replace("{NameSpace}", item.NameSpace);
				contents = contents.Replace("{ModelName}", modelName);
				contents = contents.Replace("{ModelNameSpace}", "Model");
				contents = contents.Replace("{ModelNamePluralized}", pluralizationHelper.Pluralize(modelName));
				contents = contents.Replace("{PKType}", PKType);
				contents = contents.Replace("{BaseModelName}", BaseModelName);
				contents = contents.Replace("{instanceModel}", instanceModel);

				//prettify cs code
				contents = PrettifyRoslyn(contents);
			
				//save file to disk
				fileHelper.SaveFileToDisk(contents, path, item.FolderName, item.Prefix + modelName + item.Suffix + ".cs");
			}
			Console.WriteLine("All Done ");
			Console.ReadLine();
		}

		/// <summary>
		/// Arrange Using Roslyn
		/// </summary>
		/// <param name="csCode">csCode string</param>
		/// <returns></returns>
		private static string PrettifyRoslyn(string csCode)
		{
			SyntaxTree tree = CSharpSyntaxTree.ParseText(csCode);
			SyntaxNode root = tree.GetRoot().NormalizeWhitespace();
			string response = root.ToFullString();
			return response;
		}
	}
}
