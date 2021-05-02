
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TextureLoader
{
	public Earth earthModel;

	public static Dictionary<int, Texture> images = new Dictionary<int, Texture>();

	public int lowerKey, lastLowerKey;
	public int higherKey, lastHigherKey;
	public float lastAlpha;

	private readonly string FILENAMEBASE = "";
	private readonly int MAXIMAGE = 44;
	//private Sphere earth;
	//private SimpleUniverse universe;
	private string fileExtension = ".jpg";
	private string filePath = "";
	private int imgAmount = 0;
	private int imgIterator = 0;
	private Texture currImage = null;


	public void loadTextures()
	{
		int[] albedoValues = {  1706, 1742, 1777, 1767, 1795, 1828,
								1858, 1881, 1886, 1906, 1960, 2047,
								2067, 2094, 2139, 2202, 2207, 2225,
								2252, 2304, 2305, 2658, 2773, 2887,
								2974, 3041, 3202, 3365, 3571, 3869,
								4170, 4366, 4692, 5040, 5321, 5646,
								5866, 6097, 6555, 7210, 8425, 9079,
								9289, 9304};

		for (int i = 1; i <= MAXIMAGE; i++)
		{
			try
			{
				images.Add(
						albedoValues[i - 1],
						LoadTextureFromFile(FILENAMEBASE + "pic"
								+ i.ToString("#00.###")));
			}
			catch (IOException e)
			{
				LogHandler.WriteMessage(e.ToString());
			}
		}
	}


	public void ComputeTextures()
	{
		int value = (int)(earthModel.getGlaciers() * 10000.0) + 800;

		int minFault = 10000;
		bool lowerKeyFound = false;
		bool higherKeyFound = false;
		foreach (int currValue in images.Keys)
		{
			int fault = value - currValue;
			if (fault < minFault && fault > 0)
			{
				lowerKey = currValue;
				minFault = fault;
				lowerKeyFound = true;
			}
		}

		minFault = 10000;
		foreach (int currValue in images.Keys)
		{
			int fault = currValue - value;
			if (fault < minFault && fault > 0)
			{
				higherKey = currValue;
				minFault = fault;
				higherKeyFound = true;
			}
		}

		if (!lowerKeyFound)
		{
			lowerKey = higherKey;
		}
		if (!higherKeyFound)
		{
			higherKey = lowerKey;
		}


		double totalDiff = higherKey - lowerKey;
		double adjustValue = value - lowerKey;
		float alphaValue = 1f;

		if (totalDiff > 0.0)
		{
			alphaValue = (float)(adjustValue / totalDiff);
		}

		if (alphaValue < 0)
		{
			alphaValue = 1f;
		}

		if (lastHigherKey != higherKey || lastLowerKey != lowerKey || lastAlpha != alphaValue)
		{
			lastHigherKey = higherKey;
			lastLowerKey = lowerKey;
			lastAlpha = alphaValue;
		}
	}

	private Texture LoadTextureFromFile(string filePath)
	{
		Texture2D tex = null;
		tex = (Texture2D)Resources.Load(filePath);
		return tex;
	}
}