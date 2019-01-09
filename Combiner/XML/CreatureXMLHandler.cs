﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Combiner
{
	public static class CreatureXMLHandler
	{
		private static readonly XNamespace ns = "IC";
		private static readonly string m_SchemaDirectory = "../../XML/Schemas";
		private static readonly string m_SavedCreaturesSchema = "SavedCreatures.xsd";

		/// <summary>
		/// Adds the creatures from the XML to the database
		/// </summary>
		public static IEnumerable<CreatureData> GetCreatureDataFromXML(string filePath)
		{
			XElement xml = GetXML(filePath);
			if (xml == null)
			{
				return new List<CreatureData>();
			}
			if (!ValidateWithSchema(xml, GetSchema()))
			{
				return new List<CreatureData>();
			}
			XDocument doc = new XDocument(xml);

			IEnumerable<CreatureData> allCreatureData =
				from creature in doc.Descendants(ns + "Creature")
				select new CreatureData()
				{
					left = creature.Element(ns + "left").Value,
					right = creature.Element(ns + "right").Value,
					bodyParts = BuildBodyParts(creature.Element(ns + "bodyParts"))
				};

			return allCreatureData;
		}

		/// <summary>
		/// Builds a dictionary of the body parts from the XML
		/// </summary>
		/// <param name="xmlBodyParts"></param>
		/// <returns></returns>
		private static Dictionary<string, string> BuildBodyParts(XElement xmlBodyParts)
		{
			Dictionary<string, string> bodyParts = new Dictionary<string, string>();
			foreach (var item in xmlBodyParts.Descendants(ns + "item"))
			{
				bodyParts.Add(item.Element(ns + "key").Value, item.Element(ns + "value").Value);
			}
			return bodyParts;
		}

		/// <summary>
		/// Adds creatures to saved creatures XML
		/// </summary>
		/// <param name="creatures"></param>
		public static void AddCreaturesToXML(IEnumerable<Creature> creatures, string filePath)
		{
			XElement xmlSavedCreatures = CreateXML(filePath);

			foreach (var creature in creatures)
			{
				XElement xmlBodyParts = new XElement(ns + "bodyParts");
				foreach (var key in creature.BodyParts.Keys)
				{
					xmlBodyParts.Add(new XElement(ns + "item",
						new XElement(ns + "key", key),
						new XElement(ns + "value", creature.BodyParts[key])));
				}

				XElement xmlCreature = new XElement(ns + "Creature");
				xmlCreature.Add(new XElement(ns + "left", creature.Left),
					new XElement(ns + "right", creature.Right),
					xmlBodyParts);

				xmlSavedCreatures.Add(xmlCreature);
			}

			SaveXml(xmlSavedCreatures, filePath);
		}

		/// <summary>
		/// Adds the creature to the saved creatures XML
		/// </summary>
		/// <param name="creature"></param>
		private static void AddCreatureToXML(Creature creature, string filePath)
		{
			XElement xmlSavedCreatures = CreateXML(filePath);

			XElement xmlBodyParts = new XElement(ns + "bodyParts");
			foreach (var key in creature.BodyParts.Keys)
			{
				xmlBodyParts.Add(new XElement(ns + "item",
					new XElement(ns + "key", key),
					new XElement(ns + "value", creature.BodyParts[key])));
			}

			XElement xmlCreature = new XElement(ns + "Creature");
			xmlCreature.Add(new XElement(ns + "left", creature.Left),
				new XElement(ns + "right", creature.Right),
				xmlBodyParts);

			xmlSavedCreatures.Add(xmlCreature);
			SaveXml(xmlSavedCreatures, filePath);
		}

		/// <summary>
		/// Saves the given XML
		/// </summary>
		/// <param name="xml"></param>
		private static void SaveXml(XElement xml, string filePath)
		{
			if (xml == null)
			{
				return;
			}

			if (!File.Exists(filePath))
			{
				return;
			}

			if (ValidateWithSchema(xml, GetSchema()))
			{
				XDocument doc = new XDocument(xml);
				doc.Save(filePath);
			}
		}

		/// <summary>
		/// Validates the XML using the given schema
		/// </summary>
		/// <param name="xElement"></param>
		/// <param name="schemas"></param>
		/// <returns></returns>
		private static bool ValidateWithSchema(XElement xElement, XmlSchemaSet schemas)
		{
			string errorMessage = string.Empty;
			bool result = false;

			try
			{
				if (xElement != null && schemas != null)
				{
					XDocument xmlDoc = new XDocument(xElement);
					xmlDoc.Validate(schemas, (sender, eventArgs) => { errorMessage = eventArgs.Message; });
				}
				else
				{
					errorMessage = "Null xml or schema";
				}

				if (string.IsNullOrWhiteSpace(errorMessage))
				{
					result = true;
				}
			}
			catch (XmlSchemaValidationException ex)
			{
				errorMessage = ex.Message;
			}

			Console.WriteLine(errorMessage);

			return result;
		}

		/// <summary>
		/// Creates the saved creatures XML file
		/// </summary>
		/// <returns></returns>
		private static XElement CreateXML(string filePath)
		{
			if (!File.Exists(filePath))
			{
				Stream sr = File.Create(filePath);
				sr.Close();
				return new XElement(ns + "SavedCreatures");
			}

			XElement xml;
			using (Stream sr = File.Open(filePath, FileMode.Open))
			{
				xml = XElement.Load(sr);
			}

			return xml;
		}

		/// <summary>
		/// Gets the XML from the saved creatures XML file
		/// </summary>
		/// <returns></returns>
		private static XElement GetXML(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return null;
			}

			XElement xml;
			using (Stream sr = File.Open(filePath, FileMode.Open))
			{
				xml = XElement.Load(sr);
			}

			return xml;
		}


		/// <summary>
		/// Gets the saved creatures XML schema
		/// </summary>
		/// <returns></returns>
		private static XmlSchemaSet GetSchema()
		{
			if (!Directory.Exists(m_SchemaDirectory))
			{
				return null;
			}

			XmlSchemaSet schemas = new XmlSchemaSet();
			schemas.Add("IC", Path.Combine(m_SchemaDirectory, m_SavedCreaturesSchema));
			return schemas;
		}

		
	}
}
