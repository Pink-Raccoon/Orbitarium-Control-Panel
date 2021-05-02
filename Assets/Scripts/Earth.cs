using System;
using UnityEngine;
using UnityEngine.UI;

public class Earth : MonoBehaviour
{
	public Glaciers Glaciers;
	public Text TempText;
	public Text SeaLevelText;
	public Text YearText;

	public RenderTexture RenderTexture;
	private bool IsPlaying = false;
	// Rendering
	private Material mat;
	// Textures
	private TextureLoader textureLoader;

	private double ppm; // CO2 contingent [0..1000]
	private double eqTemp; // equilibrium temperature
	private double actTemp; // actual temperature
	private double actAlbedo; // actual albedo
	private double actClouds;
	private const double ALLCLOUDS = 65; // temp whole sky with clouds
	private const double EVAPORATION = 0.2;
	private const double ALBEDOCLOUD = 0.7;
	private const double ALBEDOGROUND = 0.4;
	private const double POWERSUN = 343;
	private const double GREENHOUSECLOUD = .9;
	private const double GREENHOUSECO2 = 0.4 / 280;

	private int year;

	//private PropertyChangeSupport props; 
	public delegate void OnVariableChangeDelegate(double newVal);
	public event OnVariableChangeDelegate OnVariableChange;

	public void Start()
	{
		reset();
		Glaciers.reset();

		//props = new PropertyChangeSupport(this);
		mat = new Material(Shader.Find("Co2Shader"));
		textureLoader = new TextureLoader();
		textureLoader.earthModel = this;
		textureLoader.loadTextures();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (!IsPlaying)
		{
			return;
		}

		try
		{
			calcTemp();
			year += 10;

			// Update GUI
			TempText.text = Decimal.Round((Decimal)getTemp(), 2).ToString();
			SeaLevelText.text = Decimal.Round((Decimal)getSeaLevel(), 2).ToString();
			YearText.text = year.ToString();

			// Render
			textureLoader.ComputeTextures();

			mat.SetTexture("_Texture2", TextureLoader.images[textureLoader.lastHigherKey]);
			mat.SetFloat("_Alpha", textureLoader.lastAlpha);

			Graphics.Blit(TextureLoader.images[textureLoader.lastLowerKey], RenderTexture, mat);
		}
		catch (Exception e)
		{
			LogHandler.WriteMessage(e.ToString());
		}
	}

	public void PlayPause()
	{
		IsPlaying = !IsPlaying;
	}

	public void setPpm(double v)
	{
		ppm = v;
	}

	// power that arrives ground
	public double getPowerGround()
	{
		// reflected at the clouds
		double powerGround = POWERSUN * (1 - getClouds() * ALBEDOCLOUD);
		double ground = 1 - Glaciers.getGlaciers();
		// reflected at ground
		powerGround *= ground * (1 - ALBEDOGROUND) + Glaciers.getGlaciers() * (1 - Glaciers.ALBEDOICE);
		return round2(powerGround);
	}

	// power due to greenhouse effect
	public double getPowerGreenHouse()
	{
		double pg = getPowerGround();
		return round2(pg * getClouds() * GREENHOUSECLOUD + pg * ppm * GREENHOUSECO2);
	}

	/* clouds sea in [0..1]
	 * -5 -> 0
	 * 14 -> 0.1/0.86/0.2
	 * 65 -> 1/0.86/0.2;
	 */

	public void calcClouds()
	{
		double a = .1028563200e-2;
		double b = .2134268641e-1;
		double c = .8099935203e-1;
		double openSea = 1 - Glaciers.getGlaciers();
		if (actTemp <= Glaciers.ALLFREEZE) actClouds = 0;
		else if (actTemp >= ALLCLOUDS) actClouds = 1;
		else
		{
			actClouds = (a * actTemp * actTemp + b * actTemp + c) * openSea * EVAPORATION;
		}
		actClouds = round2(actClouds);
	}

	public double getPpm()
	{
		return ppm;
	}

	public double getClouds()
	{
		return actClouds;
	}

	public double getSeaLevel()
	{
		// 0 -> 80
		// 0.16 -> 0
		// 0.30 -> -120	
		// 1 -> -600
		double a = 1394.557823;
		double b = -1831.972789;
		double c = -242.5850340;
		double d = 80.0;
		double g = Glaciers.getGlaciers();
		return Math.Max(-600, a * g * g * g + b * g * g + c * g + d);
	}


	public double getGlaciers()
	{
		return Glaciers.getGlaciers();
	}

	public int getYear()
	{
		return year;
	}

	public void reset()
	{
		eqTemp = 14;
		actTemp = 14;
		ppm = 280;
		year = 1870;
		Glaciers.reset();
	}

	private void update()
	{
		//props.firePropertyChange("temp", -273, actTemp);
		OnVariableChange(actTemp);
	}

	public double getTemp() { return actTemp; }
	/*
	public void addPropertyChangeListener(PropertyChangeListener list)
	{
		props.addPropertyChangeListener(list);
	}
	*/

	private double round2(double d)
	{
		return (int)(d * 100) / 100.0;
	}

	// ppm to temp in equilibrium function
	private double ppm2temp(double ppm)
	{
		return ppm / 20;
	}

	/*
	  * calculates temperature change considering
	  * actual temperatur, albedo, clouds
	  */
	private double tempChange()
	{
		// very simple implementation
		return (eqTemp - actTemp) / 100;
	}

	private void calcTemp()
	{
		if (ppm > 100 && ppm < 500 && actTemp > 5 && actTemp < 25)
		{
			// stable
			// 100 ppm -> 5
			// 280 ppm -> 14
			// 500 ppm -> 25
			double a = .1390025253e-4;
			double b = .3916234848e-1;
			double c = 1.944762626;
			eqTemp = a * ppm * ppm + b * ppm + c;
			//System.out.println("stable: "+eqTemp+" "+actTemp);
		}
		else
		{
			// runaway
			// pwr 170 -> -18
			// pwr 240 -> 14
			// pwr 420 -> 35
			double a = -.1154584546e-2;
			double b = .8872272944;
			double c = -133.9667498;
			double pwr = getPowerGround() + getPowerGreenHouse();
			eqTemp = a * pwr * pwr + b * pwr + c;
			//System.out.println("runaway: "+eqTemp+" "+actTemp);
		}
		eqTemp = Math.Max(eqTemp, -50);
		actTemp = round2(actTemp + tempChange());


		calcClouds();
		if (this.year % 500 == 0)
		{
			Glaciers.setTemp(actTemp);
		}
	}


	/*
     * calculates glaciers change considering
     * actual temperatur, albedo, clouds

    private void calcGlaciers() {
    	// very simple dummy implementation
        actGlaciers = Math.max(0,1-(actTemp*6)/100);
    }
    */


}
