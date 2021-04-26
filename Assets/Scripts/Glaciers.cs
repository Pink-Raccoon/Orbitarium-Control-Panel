using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glaciers : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    void FixedUpdate()
    {
        try
        {
            calcGlaciers();
        }
        catch (Exception e)
        {
            e.ToString();
        }
    }


    private double eqGlaciers; // equilibrium glaciers [0..1]
    private double actGlaciers; // actual glaciers [0..1]
    private double actTemp;
    public readonly static double ALBEDOICE = 0.9;
    public readonly static double ALLFREEZE = -5;
    public readonly static double ALLMELT = 25;
    private static double ICETIMECONSTANT = 10;

    public void setTemp(double temp)
    {
        actTemp = temp;
    }


    public double getGlaciers()
    {
        return round3(actGlaciers);
    }

    public void reset()
    {
        setTemp(14);
        eqGlaciers = 0.16;
        actGlaciers = 0.16;
    }

    /*
     * temp < -5 -> 100% glaciers
     * 14 -> 16%
     * > 25 -> 0%
     */
    public void calcGlaciers()
    {
        double c = .7097288676;
        double b = -.5311004785e-1;
        double a = .9888357257e-3;
        if (actTemp <= ALLFREEZE) eqGlaciers = 1;
        else if (actTemp >= ALLMELT) eqGlaciers = 0;
        else
        {
            eqGlaciers = (a * actTemp * actTemp + b * actTemp + c);
        }
        actGlaciers = actGlaciers + glacierChange();
        // System.out.println(actTemp+" "+actGlaciers+" "+eqGlaciers);

    }
    private double glacierChange()
    {
        if (actTemp > ALLFREEZE) return (eqGlaciers - actGlaciers) / 300;
        else return (eqGlaciers - actGlaciers) / 10 * -actTemp / 300;
    }


    private double round3(double d)
    {
        return (int)(d * 1000 + 0.5) / 1000.0;
    }
}
