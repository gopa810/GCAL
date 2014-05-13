﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPAppDayResults
    {
        public GPLocationProvider location = null;
        public GPGregorianTime evente;
        public GPAstroData details = new GPAstroData();
        public Boolean b_adhika;

        public List<GPStringPair> output = new List<GPStringPair>();

        public void calculateAppearanceDayData(GPLocationProvider aLocation, GPGregorianTime aEvente)
        {
            //MOONDATA moon;
            //SUNDATA sun;
            location = aLocation;
            evente = new GPGregorianTime(aEvente);
            double dd;
            GPAstroData d = details;
            GPGregorianTime vc = evente;
            GPGregorianTime vcsun = evente;

            b_adhika = false;

            //d.nTithi = GetPrevTithiStart(m_earth, vc, dprev);
            //GetNextTithiStart(m_earth, vc, dnext);
            //vcsun.setDayHours(vcsun.getDayHours() - vcsun.getTimeZoneOffsetHours() / 24.0);
            vcsun.normalizeValues();
            d.sun.calculateCoordinates(vcsun, -1);
            d.moon.MoonCalc(vcsun.getJulianGreenwichTime());
            d.msDistance = GPMath.putIn360(d.moon.longitude_deg - d.sun.eclipticalLongitude - 180.0);
            d.msAyanamsa = GPAyanamsa.GetAyanamsa(vc.getJulianGreenwichTime());

            // tithi
            dd = d.msDistance / 12.0;
            d.nTithi = Convert.ToInt32(Math.Floor(dd));
            d.nTithiElapse = GPMath.frac(dd) * 100.0;
            d.nPaksa = (d.nTithi >= 15) ? 1 : 0;


            // naksatra
            dd = GPMath.putIn360(d.moon.longitude_deg - d.msAyanamsa);
            dd = (dd * 3.0) / 40.0;
            d.nNaksatra = Convert.ToInt32(Math.Floor(dd));
            d.nNaksatraElapse = GPMath.frac(dd) * 100.0;
            d.nMasa = d.determineMasa(vc, out d.nGaurabdaYear);
            d.nMoonRasi = GPEngine.GetRasi(d.moon.longitude_deg, d.msAyanamsa);
            d.nSunRasi = GPEngine.GetRasi(d.sun.eclipticalLongitude, d.msAyanamsa);

            if (d.nMasa == GPMasa.ADHIKA_MASA)
            {
                d.nMasa = GPEngine.GetRasi(d.sun.eclipticalLongitude, d.msAyanamsa);
                b_adhika = true;
            }

            List<string> gstr = GPStrings.getSharedStrings().gstr;
            output.Add(new GPStringPair(gstr[25], "", true));
            output.Add(new GPStringPair(gstr[7], vc.ToString()));
            output.Add(new GPStringPair(gstr[8], vc.getShortTimeString()));
            output.Add(new GPStringPair(gstr[9], vc.getLocation().getCity()));
            output.Add(new GPStringPair(gstr[10], vc.getLocation().getLatitudeString()));
            output.Add(new GPStringPair(gstr[11], vc.getLocation().getLongitudeString()));
            output.Add(new GPStringPair(gstr[12], vc.getLocation().getTimeZoneName()));
            output.Add(new GPStringPair(gstr[1001], "N/A"));
            output.Add(new GPStringPair(gstr[13], GPTithi.getName(d.nTithi)));
            output.Add(new GPStringPair(gstr[14], string.Format("{0:0.###} %", d.nTithiElapse)));
            output.Add(new GPStringPair(gstr[15], GPNaksatra.getName(d.nNaksatra)));
            output.Add(new GPStringPair(gstr[16], string.Format("{0:0.###} % ({1})", d.nNaksatraElapse, gstr[811 + Convert.ToInt32(d.nNaksatraElapse / 25.0)])));
            output.Add(new GPStringPair(gstr[991], GPSankranti.getName(d.nMoonRasi)));
            output.Add(new GPStringPair(gstr[992],  GPSankranti.getName(d.nSunRasi)));
            output.Add(new GPStringPair(gstr[20], GPPaksa.getName(d.nPaksa)));
            if (b_adhika == true)
                output.Add(new GPStringPair(gstr[22], string.Format("{0} {1}", GPMasa.GetName(d.nMasa), gstr[21])));
            else
                output.Add(new GPStringPair(gstr[22], GPMasa.GetName(d.nMasa)));
            output.Add(new GPStringPair(gstr[23], d.nGaurabdaYear.ToString()));

            if (GPDisplays.AppDay.childNameSuggestions())
            {
                output.Add(new GPStringPair());
                output.Add(new GPStringPair(gstr[17], "", true));
                output.Add(new GPStringPair());
                output.Add(new GPStringPair(gstr[18], string.Format("{0}...", GPAppHelper.GetNaksatraChildSylable(d.nNaksatra, Convert.ToInt32(d.nNaksatraElapse / 25.0)))));
                output.Add(new GPStringPair(gstr[19], string.Format("{0}...", GPAppHelper.GetRasiChildSylable(d.nMoonRasi))));
            }

            vc.Today();
            GPVedicTime va = new GPVedicTime();
            GPGregorianTime vctemp;

            va.tithi = d.nTithi;
            va.masa = d.nMasa;
            va.gyear = GPGaurabdaYear.getGaurabdaYear(vc, location);
            if (va.gyear < d.nGaurabdaYear)
                va.gyear = d.nGaurabdaYear;

            output.Add(new GPStringPair());
            output.Add(new GPStringPair(gstr[24], "", true));
            output.Add(new GPStringPair());

            int countC = GPUserDefaults.IntForKey("appday.celebs", 3);
            int m = 0;
            for (int i = 0; i < 6; i++)
            {
                GPEngine.VATIMEtoVCTIME(va, out vctemp, location);
                if (va.gyear > d.nGaurabdaYear)
                {
                    if (m < countC)
                    {
                        output.Add(new GPStringPair(string.Format("{0} {1}", GPStrings.getSharedStrings().getString(994), va.gyear), vctemp.ToString()));
                        m++;
                    }
                }
                va.gyear++;
            }
        }

    }
}