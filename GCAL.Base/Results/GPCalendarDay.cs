﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPCalendarDay
    {
        public class Festival
        {
            public string ShowSettingItem = string.Empty;
            public string Text = string.Empty;
            private int p_fastType = 0;
            private int p_previousFastType = 0;
            public string FastSubject = null;

            /// <summary>
            /// Sortkey used for sorting results
            /// 
            /// Values are used as follows:
            /// 8       - Mahadvadasi type
            /// 10      - Ekadasi parana string
            /// 10      - ekadasi Type
            /// 11      - Total fast even .....
            /// 35-65   - event of specified class
            /// 80      - events depending on sankranti
            /// 100     - (fasting done yesterday, today ...)
            /// 200     - event of unspecified class
            /// 300-316 - various astronomical/civil events for calendar day
            /// 400-447 - caturmasya strings
            /// 448     - start/end of Bhisma pancaka
            /// 900     - default value
            /// 999     - empty line in comparision HTML report
            /// </summary>
            /// 
            public int SortKey = 900;

            public Festival()
            {
            }

            public Festival(int sortKey, string aText)
            {
                Text = aText;
                SortKey = sortKey;
            }

            public Festival(int sortKey, string aSetItem, string aText)
            {
                ShowSettingItem = aSetItem;
                Text = aText;
                SortKey = sortKey;
            }
            public Festival(int sortKey, string aSetItem, string aText, int aFast)
            {
                ShowSettingItem = aSetItem;
                Text = aText;
                setFastType(aFast);
                SortKey = sortKey;
            }
            public Festival(int sortKey, string aSetItem, string aText, int aFast, string aSubject)
            {
                ShowSettingItem = aSetItem;
                Text = aText;
                setFastType(aFast);
                FastSubject = aSubject;
                SortKey = sortKey;
            }

            public void setFastType(int value)
            {
                p_previousFastType = p_fastType;
                p_fastType = value;
            }
            public int getFastType()
            {
                return p_fastType;
            }

            public int getPreviousFastType()
            {
                return p_previousFastType;
            }
        }

        private GPLocation p_location = null;

        // date
        public GPGregorianTime date = new GPGregorianTime(GPLocation.getEmptyLocation());
        // moon times
        public GPGregorianTime moonrise = null;
        public GPGregorianTime moonset = null;
        // astronomical data from astro-sub-layer
        public GPAstroData astrodata = new GPAstroData();
        // data for vaisnava calculations
        public List<Festival> Festivals = new List<Festival>();
        public int nFastType = GPConstants.FAST_NULL;
        public int nMahadvadasiType = GPConstants.EV_NULL;
        public String sEkadasiVrataName = String.Empty;
        public GPGregorianTime ekadasiParanaStart = null;
        public GPGregorianTime ekadasiParanaEnd = null;
        public int EkadasiParanaReasonStart = GPConstants.EP_TYPE_NULL;
        public int EkadasiParanaReasonEnd = GPConstants.EP_TYPE_NULL;
        public int sankranti_zodiac = -1;
        public GPGregorianTime sankranti_day = new GPGregorianTime(GPLocation.getEmptyLocation());
        public bool IsSecondDayTithi = false;
        public GPLocalizedTithi ksayaTithi = null;
        private GPCalendarResults resultCalendar = null;
        private int indexInArray = -1;


        public int GetFastingItemIndex()
        {
            for (int i = 0; i < Festivals.Count; i++)
            {
                if (Festivals[i].getFastType() != GPConstants.FAST_NULL)
                    return i;
            }
            return -1;
        }

        public bool GetTithiTimeRange(GPLocation earth, out GPGregorianTime from, out GPGregorianTime to)
        {
            GPGregorianTime start = new GPGregorianTime(date);
            start.setDayHours(astrodata.sun.getSunriseDayHours());

            GPTithi.GetNextTithiStart(start, out to);
            GPTithi.GetPrevTithiStart(start, out from);

            return true;
        }

        public bool GetNaksatraTimeRange(GPLocation earth, out GPGregorianTime from, out GPGregorianTime to)
        {
            GPGregorianTime start = new GPGregorianTime(date);

            start.setDayHours(astrodata.sun.getSunriseDayHours());

            GPNaksatra.GetNextNaksatra(start, out to);
            GPNaksatra.GetPrevNaksatra(start, out from);

            return true;
        }

        public bool FindText(string text)
        {
            foreach (Festival fest in Festivals)
            {
                if (fest.Text.IndexOf(text) >= 0)
                    return true;
                if (fest.FastSubject != null && fest.FastSubject.IndexOf(text) >= 0)
                    return true;
            }
            return false;
        }

        public Festival FindShowSettingItem(string item)
        {
            foreach (Festival fest in Festivals)
            {
                if (fest.ShowSettingItem.CompareTo(item) == 0)
                    return fest;
            }
            return null;
        }

        public string getTithiNameExtended()
        {
            if ((astrodata.nTithi == 10) || (astrodata.nTithi == 25)
                || (astrodata.nTithi == 11) || (astrodata.nTithi == 26))
            {
                if (hasEkadasiParana() == false)
                {
                    if (nMahadvadasiType == GPConstants.EV_NULL)
                    {
                        return string.Format("{0} {1}", GPTithi.getName(astrodata.nTithi), GPStrings.getString(58));
                    }
                    else
                    {
                        return string.Format("{0} {1}", GPTithi.getName(astrodata.nTithi), GPStrings.getString(59));
                    }
                }
            }

            return GPTithi.getName(astrodata.nTithi);
        }

        public List<Festival> CompleteFestivalList(GPCalendarDay prevDay, GPCalendarDay nextDay)
        {
            List<Festival> fests = new List<Festival>();

            if (hasEkadasiParana())
            {
                fests.Add(new Festival(10, getEkadasiParanaString()));
            }


            foreach (Festival fest in Festivals)
            {
                fests.Add(fest);
            }

            if (sankranti_zodiac >= 0)
            {
                fests.Add(new Festival(300, GPDisplays.Keys.CalendarSankranti, string.Format(GPStrings.getString(975), GPSankranti.getName(sankranti_zodiac), sankranti_day.ToString(), sankranti_day.getLongTimeString())));
            }

            if (hasKsayaTithi())
            {
                fests.Add(new Festival(301, GPDisplays.Keys.CalendarKsaya, string.Format(GPStrings.getString(976), ksayaTithi.getName(), getKsayaTimeString(0), getKsayaTimeString(1))));
            }

            if (IsSecondDayTithi)
            {
                fests.Add(new Festival(302, GPDisplays.Keys.CalendarVriddhi, GPStrings.getString(977)));
            }

            // tithi at arunodaya
            if (GPDisplays.Calendar.TithiArunodayaVisible())
            {
                fests.Add(new Festival(303, GPDisplays.Keys.CalArunodayaTithi, string.Format("{0}: {1}", GPStrings.getString(98), GPTithi.getName(astrodata.getTithiAtArunodaya()))));
            }

            //"Arunodaya Time",//1
            if (GPDisplays.Calendar.TimeArunodayaVisible())
            {
                fests.Add(new Festival(304, GPDisplays.Keys.CalArunodayaTime, string.Format(GPStrings.getString(99), astrodata.sun.arunodaya.getShortTimeString())));
            }

            //List<string> gstr = GPStrings.getSharedStrings().gstr;

            if (GPDisplays.Calendar.TimeSunriseVisible())
            {
                fests.Add(new Festival(305, GPDisplays.Keys.CalSunriseTime, string.Format("{0} {1}", GPStrings.getString(51), astrodata.sun.rise.getShortTimeString())));
            }

            if (GPDisplays.Calendar.NoonTime())
            {
                fests.Add(new Festival(306, GPDisplays.Keys.CalNoonTime, string.Format("{0} {1}", GPStrings.getString(857), astrodata.sun.noon.getShortTimeString())));
            }

            if (GPDisplays.Calendar.TimeSunsetVisible())
            {
                fests.Add(new Festival(307, GPDisplays.Keys.CalSunsetTime, string.Format("{0} {1}", GPStrings.getString(52), astrodata.sun.set.getShortTimeString())));
            }

            if (GPDisplays.Calendar.TimeMoonriseVisible())
            {
                if (moonrise != null)
                {
                    fests.Add(new Festival(308, GPDisplays.Keys.CalMoonriseTime, string.Format("{0} {1}", GPStrings.getString(53), moonrise.getShortTimeString())));
                }
            }

            if (GPDisplays.Calendar.TimeMoonsetVisible())
            {
                if (moonset != null)
                {
                    fests.Add(new Festival(309, GPDisplays.Keys.CalMoonsetTime, string.Format("{0} {1}", GPStrings.getString(54), moonset.getShortTimeString())));
                }
            }

            if (GPDisplays.Calendar.SunLongitudeVisible())
            {
                fests.Add(new Festival(310, GPDisplays.Keys.CalSunLong, string.Format("{0}: {1} (*)", GPStrings.getString(100), astrodata.sun.rise.eclipticalLongitude)));
            }

            if (GPDisplays.Calendar.MoonLongitudeVisible())
            {
                fests.Add(new Festival(311, GPDisplays.Keys.CalMoonLong, string.Format("{0}: {1} (*)", GPStrings.getString(101), astrodata.moon.longitude_deg)));
            }

            if (GPDisplays.Calendar.AyanamsaValueVisible())
            {
                fests.Add(new Festival(312, GPDisplays.Keys.CalAyanamsa, string.Format("{0} {1} ({2}) (*)", GPStrings.getString(102), astrodata.msAyanamsa, GPAyanamsa.CurrentName)));
            }

            if (GPDisplays.Calendar.JulianDayVisible())
            {
                fests.Add(new Festival(313, GPDisplays.Keys.CalJulian, string.Format("{0} {1} (*)", GPStrings.getString(103), astrodata.jdate)));
            }

            if (GPDisplays.Calendar.StartMasaVisible())
            {
                if (prevDay != null)
                {
                    if (prevDay.astrodata.nMasa != this.astrodata.nMasa)
                    {
                        fests.Add(new Festival(314, GPDisplays.Keys.CalMasaChange, string.Format("{0} {1} {2}", GPStrings.getString(780), GPMasa.GetName(astrodata.nMasa), GPStrings.getString(22))));
                    }
                }
                if (nextDay != null)
                {
                    if (nextDay.astrodata.nMasa != this.astrodata.nMasa)
                    {
                        fests.Add(new Festival(315, GPDisplays.Keys.CalDstChange, string.Format("{0} {1} {2}", GPStrings.getString(781), GPMasa.GetName(astrodata.nMasa), GPStrings.getString(22))));
                    }
                }
            }

            if (GPDisplays.Calendar.DSTNotice())
            {
                if (prevDay != null && prevDay.isDaylightInEffect() == 0 && this.isDaylightInEffect() == 1)
                    fests.Add(new Festival(316, GPDisplays.Keys.CalDstChange, GPStrings.getString(855)));

                if (nextDay != null && this.isDaylightInEffect() == 1 && nextDay.isDaylightInEffect() == 0)
                    fests.Add(new Festival(316, GPDisplays.Keys.CalDstChange, GPStrings.getString(856)));
            }
            return fests;
        }

        public string getGaurabdaYearLongString()
        {
            return string.Format("{0} {1}", GPStrings.getString(994), astrodata.nGaurabdaYear);
        }

        public string getMasaLongName()
        {
            return string.Format("{0} {1}", GPMasa.GetName(astrodata.nMasa), GPStrings.getString(22));
        }

        public string getNaksatraElapsedString()
        {
            return string.Format("{0:0.###} %", astrodata.nNaksatraElapse);
        }

        public int getNaksatraPada()
        {
                return Convert.ToInt32(astrodata.nNaksatraElapse / 25) % 4;
        }

        public string getNaksatraName()
        {
            return GPNaksatra.getName(astrodata.nNaksatra);
        }

        public string getTithiName()
        {
            return GPTithi.getName(astrodata.nTithi);
        }

        public string getPaksaName()
        {
            return string.Format("{0} {1}", GPPaksa.getName(astrodata.nPaksa), GPStrings.getString(20));
        }

        public string getPaksaAbbreviation()
        {
            return GPPaksa.getAbbreviation(astrodata.nPaksa);
        }

        public string getRasiOfMoonName()
        {
            return GPSankranti.getName(astrodata.moon.GetRasi(astrodata.msAyanamsa));
        }

        public string getFastingFlag()
        {
                return ((nFastType != GPConstants.FAST_NULL) ? GPStrings.getString(49) : string.Empty);
        }

        public string getYogaName()
        {
            return GPYoga.getName(astrodata.nYoga);
        }

        public string getEkadasiParanaString()
        {
            string str;
            string dstIndicator = string.Empty;
            string dstIndicator2 = string.Empty;
            string shortTime = "";

            if (ekadasiParanaEnd != null)
            {
                if (GPDisplays.Calendar.EkadasiParanaDetails())
                {
                    shortTime = ekadasiParanaStart.getShortTimeString(false, ref dstIndicator);
                    str = string.Format("{0} {1} ({2}) - {3} ({4})", GPStrings.getString(60),
                        shortTime, GPAppHelper.GetParanaReasonText(EkadasiParanaReasonStart),
                        ekadasiParanaEnd.getShortTimeString(false, ref dstIndicator2), GPAppHelper.GetParanaReasonText(EkadasiParanaReasonEnd));
                }
                else
                {
                    shortTime = ekadasiParanaStart.getShortTimeString(false, ref dstIndicator);
                    str = string.Format("{0} {1} - {2}", GPStrings.getString(60),
                        shortTime, ekadasiParanaEnd.getShortTimeString(false, ref dstIndicator2));
                }
                if (!String.IsNullOrEmpty(dstIndicator))
                    str += " " + dstIndicator;
            }
            else if (ekadasiParanaStart != null)
            {
                if (GPDisplays.Calendar.EkadasiParanaDetails())
                    str = string.Format("{0} {1} ({2})", GPStrings.getString(61),
                        ekadasiParanaStart.getShortTimeString(), GPAppHelper.GetParanaReasonText(EkadasiParanaReasonStart));
                else
                    str = string.Format("{0} {1}", GPStrings.getString(61),
                        ekadasiParanaStart.getShortTimeString());
            }
            else
            {
                str = GPStrings.getString(62);
            }

            return str;
        }

        public string getWeekdayAbbr()
        {
            return GPStrings.getString(date.getDayOfWeek()).Substring(2);
        }

        public int isDaylightInEffect()
        {
            return date.getDaylightTimeON() ? 1 : 0;
        }

        public string getKsayaTimeString(int index)
        {
            if (ksayaTithi == null)
                return string.Empty;

            if (index == 0)
            {
                GPGregorianTime vc = ksayaTithi.getStartTime();
                return string.Format("{0}, {1}", vc.ToString(), vc.getShortTimeString());
            }
            else
            {
                GPGregorianTime vc = ksayaTithi.getEndTime();
                return string.Format("{0}, {1}", vc.ToString(), vc.getShortTimeString());
            }
        }

        public void Clear()
        {
            // init
            Festivals.Clear();
            nFastType = GPConstants.FAST_NULL;
            //nFeasting = GPConstants.FEAST_NULL;
            nMahadvadasiType = GPConstants.EV_NULL;
            sEkadasiVrataName = string.Empty;
            ekadasiParanaStart = null;
            ekadasiParanaEnd = null;
            sankranti_zodiac = -1;
            sankranti_day.Clear();
            IsSecondDayTithi = false;
        }


        public bool hasEkadasiParana()
        {
            return ekadasiParanaStart != null;
        }

        public bool hasKsayaTithi()
        {
            return ksayaTithi != null;
        }

        // flag for validity
        public GPCalendarDay(GPLocation aLoc, GPCalendarResults res, int nIndex)
        {
            p_location = aLoc;
            resultCalendar = res;
            indexInArray = nIndex;
        }


        public GPCalendarDay GetDayWithOffset(int i)
        {
            if (resultCalendar != null && indexInArray >= 0)
            {
                return resultCalendar.get(indexInArray + i);
            }
            return null;
        }

        public void AddFestivalCopy(GPEvent pEvx)
        {
            GPCalendarDay.Festival fest = null;
            if (pEvx.hasFasting())
            {
                fest = new GPCalendarDay.Festival(((pEvx.nClass >= 0) ? pEvx.nClass*5 + 30 : 200), GPDisplays.Keys.FestivalClass(pEvx.nClass), pEvx.getText(), pEvx.getFastType(), pEvx.getFastSubject());
            }
            else
            {
                fest = new GPCalendarDay.Festival(((pEvx.nClass >= 0) ? pEvx.nClass*5 + 30 : 200), GPDisplays.Keys.FestivalClass(pEvx.nClass), pEvx.getText());
            }
            Festivals.Add(fest);

            if (GPDisplays.Calendar.AnniversaryType() != 2 && pEvx.nStartYear > -7000)
            {
                int years = astrodata.nGaurabdaYear - (pEvx.nStartYear - 1496);
                fest.Text += " ";
                if (GPDisplays.Calendar.AnniversaryType() == 0)
                {
                    fest.Text += string.Format(GPStrings.getString(962), years);
                }
                else
                {
                    fest.Text += string.Format(GPStrings.getString(963), years);
                }

            }

            if (pEvx.hasChildrenItems())
            {
                foreach (GPEvent ev in pEvx.childrenItems)
                {
                    if (ev is GPEventRelative)
                    {
                        GPEventRelative relev = ev as GPEventRelative;
                        GPCalendarDay vd = GetDayWithOffset(relev.nOffset);
                        if (vd != null)
                        {
                            vd.AddFestivalCopy(relev);
                        }
                    }
                }
            }

        }

        public GPLocalizedTithi getCurrentTithi()
        {
            return new GPLocalizedTithi(p_location, getSunriseTime(), astrodata.nTithi);
        }

        public GPLocalizedNaksatra getCurrentNaksatra()
        {
            return new GPLocalizedNaksatra(p_location, getSunriseTime(), astrodata.nNaksatra);
        }

        public GPGregorianTime getSunriseTime()
        {
            GPGregorianTime vc = new GPGregorianTime(this.date);
            vc.setDayHours(astrodata.sun.getSunriseDayHours());
            return vc;
        }

        public void SortFestivals()
        {
            if (Festivals != null && Festivals.Count > 1)
            {
                Festival a;
                Festival[] arr = Festivals.ToArray();
                int i, j, l = arr.Length;
                for (i = 0; i < l - 1; i++)
                {
                    for (j = i + 1; j < l; j++)
                    {
                        if (arr[i].SortKey > arr[j].SortKey)
                        {
                            a = arr[i];
                            arr[i] = arr[j];
                            arr[j] = a;
                        }
                    }
                }
                Festivals.Clear();
                Festivals.AddRange(arr);
            }
        }
    }
}
