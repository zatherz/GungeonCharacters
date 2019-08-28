﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SGUI;
using UnityEngine;
using System.Reflection;
using System.Diagnostics;

namespace CustomCharacters
{
    public static class ResourceExtractor
    {
        private static string spritesDirectory = Path.Combine(ETGMod.ResourcesDirectory, "sprites");
        private static string nameSpace = "Gungeon";
        /// <summary>
        /// Converts all png's in a folder to a list of Texture2D objects
        /// </summary>
        public static List<Texture2D> GetTexturesFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Tools.PrintError(directoryPath + " not found.");
                return null;
            }

            List<Texture2D> textures = new List<Texture2D>();
            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                if (!filePath.EndsWith(".png")) continue;

                Texture2D texture = BytesToTexture(File.ReadAllBytes(filePath), Path.GetFileName(filePath).Replace(".png", ""));
                textures.Add(texture);
            }
            return textures;
        }

        /// <summary>
        /// Creates a Texture2D from a file in the sprites directory
        /// </summary>
        public static Texture2D GetTextureFromFile(string fileName)
        {
            fileName = fileName.Replace(".png", "");
            string filePath = Path.Combine(spritesDirectory, fileName + ".png");
            if (!File.Exists(filePath))
            {
                Tools.PrintError(filePath + " not found.");
                return null;
            }
            Texture2D texture = BytesToTexture(File.ReadAllBytes(filePath), fileName);
            return texture;
        }

        /// <summary>
        /// Retuns a list of sprite collections in the sprite folder
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCollectionFiles()
        {
            List<string> collectionNames = new List<string>();
            foreach (string filePath in Directory.GetFiles(spritesDirectory))
            {
                if (filePath.EndsWith(".png"))
                {
                    collectionNames.Add(Path.GetFileName(filePath).Replace(".png", ""));
                }
            }
            return collectionNames;
        }

        /// <summary>
        /// Converts a byte array into a Texture2D
        /// </summary>
        public static Texture2D BytesToTexture(byte[] bytes, string resourceName)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(texture, bytes);
            texture.filterMode = FilterMode.Point;
            texture.name = resourceName;
            return texture;
        }

        public static string[] GetLinesFromEmbeddedResource(string filePath)
        {
            filePath = filePath.Replace("/", ".");
            filePath = filePath.Replace("\\", ".");
            string allLines = BytesToString(ExtractEmbeddedResource($"{nameSpace}." + filePath));
            return allLines.Split('\n');
        }

        public static string[] GetLinesFromFile(string filePath)
        {
            string allLines = BytesToString(File.ReadAllBytes(filePath));
            return allLines.Split('\n');
        }

        public static string BytesToString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Returns a list of folders in the ETG resources directory
        /// </summary>
        public static List<String> GetResourceFolders()
        {
            List<String> dirs = new List<String>();
            string spritesDirectory = Path.Combine(ETGMod.ResourcesDirectory, "sprites");

            if (Directory.Exists(spritesDirectory))
            {
                foreach (String directory in Directory.GetDirectories(spritesDirectory))
                {
                    dirs.Add(Path.GetFileName(directory));
                }
            }
            return dirs;
        }

        /// <summary>
        /// Converts an embedded resource to a byte array
        /// </summary>
        public static byte[] ExtractEmbeddedResource(String filename)
        {
            var baseAssembly = Assembly.GetCallingAssembly();
            using (Stream resFilestream = baseAssembly.GetManifestResourceStream(filename))
            {
                if (resFilestream == null)
                {
                    return null;
                }
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }

        /// <summary>
        /// Converts an embedded resource to a Texture2D object
        /// </summary>
        public static Texture2D GetTextureFromResource(string resourceName)
        {
            string file = resourceName;
            file = file.Replace("/", ".");
            file = file.Replace("\\", ".");
            byte[] bytes = ExtractEmbeddedResource($"{nameSpace}." + file);
            if (bytes == null)
            {
                Tools.PrintError("No bytes found in " + file);
                return null;
            }
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(texture, bytes);
            texture.filterMode = FilterMode.Point;
    
            string name = file.Substring(0, file.LastIndexOf('.'));
            if (name.LastIndexOf('.') >= 0)
            {
                name = name.Substring(name.LastIndexOf('.') + 1);
            }
            texture.name = name;

            return texture;
        }


        /// <summary>
        /// Returns a list of the names of all embedded resources
        /// </summary>
        public static string[] GetResourceNames()
        {
            var baseAssembly = Assembly.GetCallingAssembly();
            string[] names = baseAssembly.GetManifestResourceNames();
            if (names == null)
            {
                ETGModConsole.Log("No manifest resources found.");
                return null;
            }
            return names;
        }
    }
}
