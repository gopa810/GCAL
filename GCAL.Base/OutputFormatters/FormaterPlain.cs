﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class FormaterPlain : Formatter
    {

        public static void AddListText(StringBuilder text, string pText)
        {
            text.Append(string.Empty.PadLeft(17));
            text.AppendLine(pText.TrimEnd());
        }


        public static void AddNoteText(StringBuilder builder)
        {
            builder.AppendLine();
            builder.AppendLine();
            builder.Append(gpszSeparator);
            builder.AppendLine();
            builder.AppendLine(getSharedStrings()[978]);
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine(getSharedStrings()[979]);
            builder.AppendLine(getSharedStrings()[980]);

            if (GPDisplays.Calendar.JulianDayVisible() || GPDisplays.Calendar.AyanamsaValueVisible() || GPDisplays.Calendar.SunLongitudeVisible() || GPDisplays.Calendar.MoonLongitudeVisible())
            {
                builder.AppendLine(getSharedStrings()[981]);
            }

            // last line
            builder.AppendLine();
            builder.AppendFormat(getSharedStrings()[982], GPAppHelper.getShortVersionText());
            builder.Append(GPAppHelper.getShortVersionText());
        }

        public static void AddTextLine(StringBuilder text, string added)
        {
            text.AppendLine(added);
        }

        public static void AddListText(StringBuilder text, string pText, string pText2)
        {
            text.AppendFormat("{0}{1}", pText.PadRight(17), pText2.TrimEnd());
            text.AppendLine();
        }

        public static int FormatEventsText(GPCoreEventResults inEvents, StringBuilder res)
        {

            int i;

            res.AppendFormat(getSharedStrings()[983], inEvents.m_vcStart, inEvents.m_vcEnd);
            res.AppendLine();
            res.AppendLine();

            res.Append(inEvents.m_location.getLocation(0).getFullName());
            res.AppendLine();
            res.AppendLine();

            DateTime prevd = new DateTime(1970, 1, 1);
            int prevt = -1;

            for (i = 0; i < inEvents.getCount(); i++)
            {
                GPCoreEvent dnr = inEvents.get(i);

                if (inEvents.b_sorted)
                {
                    DateTime dt = dnr.Time.getLocalTime();
                    if (prevd.Day != dt.Day || prevd.Month != dt.Month || prevd.Year != dt.Year)
                    {
                        res.AppendLine();
                        res.AppendLine(GPAppHelper.CenterString(dnr.Time.ToString(), 80, '='));
                        res.AppendLine();
                    }
                    prevd = dt;
                }
                else
                {
                    if (prevt != dnr.nType)
                    {
                        string s = " " + dnr.getTypeTitle() + " ";
                        res.AppendLine();
                        res.AppendLine(GPAppHelper.CenterString(s, 80, '-'));
                        res.AppendLine();
                    }
                    prevt = dnr.nType;
                }

                if (!inEvents.b_sorted)
                {
                    res.Append(dnr.Time.ToString().PadLeft(20));
                }

                res.AppendFormat("   {0}  {1}", dnr.Time.getLongTimeString(), dnr.getEventTitle());
                res.AppendLine();

            }

            res.AppendLine();

            return 1;
        }

        public static string GetTextA(GPCalendarDay pvd)
        {
            string str;
            String s1, s2;

            s1 = pvd.getTithiNameExtended();

            s2 = GPStrings.getSharedStrings().gstr[pvd.date.getDayOfWeek()].Substring(0, 2);
            str = string.Format("{0} {1} {2} {3}  {4}{5} ", pvd.date.getDay().ToString().PadLeft(2), GPAppHelper.getMonthAbr(pvd.date.getMonth()), pvd.date.getYear()
                , s2, s1.PadRight(34), (GPDisplays.Calendar.PaksaInfoVisible() ? GPPaksa.getAbbreviation(pvd.astrodata.nPaksa) : " "));

            if (GPDisplays.Calendar.YogaVisible())
            {
                str += GPYoga.getName(pvd.astrodata.nYoga).PadRight(10);
            }

            if (GPDisplays.Calendar.NaksatraVisible())
            {
                str += GPNaksatra.getName(pvd.astrodata.nNaksatra).PadRight(15);
            }

            if (pvd.nFastType != GPConstants.FAST_NULL && GPDisplays.Calendar.FastingFlagVisible())
                str += " *";
            else
                str += "  ";

            if (GPDisplays.Calendar.RasiVisible())
            {
                str += "   ";
                str += pvd.getRasiOfMoonName().PadRight(15);
            }

            return str;
        }

        public static int FormatCalendarOld(GPCalendarResults daybuff, StringBuilder m_text)
        {
            int k, nMasaHeader;
            String str;
            StringBuilder dayText = new StringBuilder();

            GPCalendarDay pvd, prevd, nextd;
            int lastmasa = -1;
            int lastmonth = -1;
            int tp1;
            bool bCalcMoon = (getShowSettingsValue(4) > 0 || getShowSettingsValue(5) > 0);

            m_text.Remove(0, m_text.Length);

            for (k = 0; k < daybuff.m_vcCount; k++)
            {

                prevd = daybuff.get(k - 1);
                pvd = daybuff.get(k);
                nextd = daybuff.get(k + 1);

                if (pvd != null)
                {
                    nMasaHeader = 0;
                    if (GPDisplays.Calendar.MasaHeader() && (pvd.astrodata.nMasa != lastmasa))
                    {
                        nMasaHeader = 1;
                        m_text.AppendLine();
                        str = string.Format("{0}, {1}", pvd.getMasaLongName(), pvd.getGaurabdaYearLongString());
                        tp1 = (80 - str.Length) / 2;
                        m_text.Append(string.Empty.PadLeft(tp1));
                        m_text.Append(str);
                        m_text.Append(string.Empty.PadLeft(tp1 - GPFileHelper.FileVersion.Length));
                        m_text.AppendLine(GPFileHelper.FileVersion);
                        if ((pvd.astrodata.nMasa == GPMasa.ADHIKA_MASA) && ((lastmasa >= GPMasa.SRIDHARA_MASA) && (lastmasa <= GPMasa.DAMODARA_MASA)))
                        {
                            AddListText(m_text, getSharedStrings()[128]);
                        }
                        m_text.AppendLine();
                        lastmasa = pvd.astrodata.nMasa;
                    }

                    if (GPDisplays.Calendar.MonthHeader() && (pvd.date.getMonth() != lastmonth))
                    {
                        nMasaHeader = 1;
                        m_text.AppendLine();
                        str = string.Format("{0} {1}", getSharedStrings()[759 + pvd.date.getMonth()], pvd.date.getYear());
                        tp1 = (80 - str.Length) / 2;
                        m_text.Append(string.Empty.PadLeft(tp1));
                        m_text.Append(str);
                        m_text.Append(string.Empty.PadLeft(tp1 - GPFileHelper.FileVersion.Length));
                        m_text.AppendLine(GPFileHelper.FileVersion);
                        lastmonth = pvd.date.getMonth();
                    }
                    if (nMasaHeader == 1)
                    {
                        m_text.AppendLine(GPAppHelper.CenterString(daybuff.m_Location.getLocation(pvd.date.getJulianGreenwichTime()).getFullName(), 80));
                        m_text.AppendLine();

                        nMasaHeader = m_text.Length;
                        m_text.Append(" ");
                        m_text.Append(getSharedStrings()[985].PadRight(16));
                        m_text.Append(getSharedStrings()[986].ToUpper().PadRight(30));
                        if (GPDisplays.Calendar.PaksaInfoVisible()) m_text.Append(getSharedStrings()[20].ToUpper().PadRight(6));
                        else m_text.Append(string.Empty.PadRight(6));
                        if (GPDisplays.Calendar.YogaVisible()) m_text.Append(getSharedStrings()[104].ToUpper().PadRight(10));
                        if (GPDisplays.Calendar.NaksatraVisible()) m_text.Append(getSharedStrings()[15].ToUpper().PadRight(15));
                        if (GPDisplays.Calendar.FastingFlagVisible()) m_text.Append(getSharedStrings()[987].ToUpper().PadRight(5));
                        if (GPDisplays.Calendar.RasiVisible()) m_text.Append(getSharedStrings()[105].ToUpper().PadRight(15));
                        nMasaHeader = m_text.Length - nMasaHeader;
                        m_text.AppendLine();
                        m_text.AppendLine(string.Empty.PadRight(nMasaHeader, '-'));
                    }

                    AvcGetOldCalendarDayText(pvd, dayText, prevd, nextd);

                    if (!GPDisplays.Calendar.HideEmptyDays() || dayText.Length > 90)
                        m_text.Append(dayText);
                }
            }

            return 1;
        }


        public static int AvcGetOldCalendarDayText(GPCalendarDay pvd, StringBuilder dayText, GPCalendarDay prevd, GPCalendarDay nextd)
        {
            String str = string.Empty, str2, str3;

            dayText.Remove(0, dayText.Length);
            str = GetTextA(pvd);
            str2 = str.Substring(16);
            str3 = str.Substring(0, 16);
            str = str3;
            if (pvd.astrodata.sun.eclipticalLongitude < 0.0)
            {
                AddListText(dayText, str, getSharedStrings()[974]);
                return 1;
            }
            AddListText(dayText, str, str2);

            foreach (GPCalendarDay.Festival fest in pvd.CompleteFestivalList(prevd, nextd))
            {

                if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                {
                    if (fest.ShowSettingItem == GPDisplays.Keys.CalendarSankranti)
                    {
                        dayText.AppendLine(GPAppHelper.CenterString(fest.Text, 80, '-'));
                    }
                    else
                    {
                        AddListText(dayText, fest.Text);
                    }
                }
            }



            return 0;
        }


        public static int FormatCalendarPlusCorePlain(GPCalendarPlusEventsResults calev, StringBuilder fout)
        {
            GPCalendarResults daybuff = calev.theCalendar;
            GPCoreEventResults events = calev.theEvents;

            List<string> gstr = GPStrings.getSharedStrings().gstr;
            int k;
            string str;
            GPGregorianTime date = new GPGregorianTime(daybuff.m_Location);
            GPCalendarDay pvd;
            GPCalendarDay prevd;
            GPCalendarDay nextd;
            int nPrevMasa = -1;
            int nPrevMonth = -1;

            if (events.b_sorted == false)
            {
                events.Sort(true);
            }

            StringBuilder lineA = new StringBuilder();
            StringBuilder lineB = new StringBuilder();


            for (k = 0; k < daybuff.getCount(); k++)
            {
                prevd = daybuff.get(k - 1);
                pvd = daybuff.get(k);
                nextd = daybuff.get(k + 1);

                if (pvd != null)
                {
                    bool writeHeaders = false;

                    if (nPrevMasa != pvd.astrodata.nMasa && GPDisplays.Calendar.MasaHeader())
                    {
                        str = string.Format("{0} {1}", pvd.getMasaLongName(), ((nPrevMasa == GPMasa.ADHIKA_MASA) ?  gstr[109] : ""));
                        fout.AppendLine(GPAppHelper.CenterString(str, 80));
                        fout.AppendLine(GPAppHelper.CenterString(pvd.getGaurabdaYearLongString(), 80));
                        fout.AppendLine(GPAppHelper.CenterString(pvd.date.getLocation().getFullName(), 80));
                        fout.AppendLine(GPAppHelper.CenterString(string.Format("{0}: {1}", gstr[12], pvd.date.getLocation().getTimeZone().getFullName()), 80));
                        fout.AppendLine();

                        nPrevMasa = pvd.astrodata.nMasa;
                        writeHeaders = true;
                    }
                    else if (nPrevMonth != pvd.date.getMonth() && GPDisplays.Calendar.MonthHeader())
                    {
                        fout.AppendLine(GPAppHelper.CenterString(string.Format("{0} {1}", GPStrings.getSharedStrings().getString(759 + pvd.date.getMonth()), pvd.date.getYear()), 80));
                        fout.AppendLine(GPAppHelper.CenterString(pvd.date.getLocation().getFullName(), 80));
                        fout.AppendLine(GPAppHelper.CenterString(string.Format("{0}: {1}", gstr[12], pvd.date.getLocation().getTimeZone().getFullName()), 80));
                        fout.AppendLine();

                        nPrevMonth = pvd.date.getMonth();
                        writeHeaders = true;
                    }


                    if (writeHeaders)
                    {
                        int len = fout.Length;

                        fout.Append(" ");
                        fout.Append(gstr[985].PadRight(16));
                        fout.Append(gstr[986].ToUpper().PadRight(30));
                        if (GPDisplays.Calendar.PaksaInfoVisible()) fout.Append(gstr[20].ToUpper().PadRight(6));
                        else fout.Append(string.Empty.PadRight(6));
                        if (GPDisplays.Calendar.YogaVisible()) fout.Append(gstr[104].ToUpper().PadRight(10));
                        if (GPDisplays.Calendar.NaksatraVisible()) fout.Append(gstr[15].ToUpper().PadRight(15));
                        if (GPDisplays.Calendar.FastingFlagVisible()) fout.Append(gstr[987].ToUpper().PadRight(5));
                        if (GPDisplays.Calendar.RasiVisible()) fout.Append(gstr[105].ToUpper().PadRight(15)); 
                        
                        fout.AppendLine();
                        len = fout.Length - len;
                        fout.AppendLine(string.Empty.PadLeft(len, '-'));
                    }

                    AvcGetOldCalendarDayText(pvd, fout, prevd, nextd);

                    fout.AppendLine();
                    lineA.Remove(0, lineA.Length);
                    lineB.Remove(0, lineB.Length);
                    List<GPStringPair> recs = events.ExtractRecordsForDate(pvd.date);
                    foreach (GPStringPair rec in recs)
                    {
                        lineA.Append(rec.Name);
                        lineB.Append(rec.Value);
                        int tosize = Math.Max(lineA.Length, lineB.Length) + 2;
                        while (lineA.Length < tosize)
                            lineA.Append(' ');
                        while (lineB.Length < tosize)
                            lineB.Append(' ');
                    }
                    fout.Append(string.Empty.PadLeft(17,' '));
                    fout.AppendLine(lineA.ToString());
                    fout.Append(string.Empty.PadLeft(17,' '));
                    fout.AppendLine(lineB.ToString());
                    fout.AppendLine();

                }
                date.setDayHours(0.0);
                date.NextDay();
            }

            return 1;
        }



        public static void FormatAppDayText(GPAppDayResults app, StringBuilder strResult)
        {
            GPAstroData d = app.details;
            string str;
            GPGregorianTime vc = app.evente;
            StringBuilder strText = strResult;

            int max = 0;
            foreach (GPStringPair rec in app.output)
            {
                if (!rec.Header)
                {
                    max = Math.Max(max, rec.Name.Length);
                }
            }
            max++;
            foreach (GPStringPair rec in app.output)
            {
                if (rec.Header)
                {
                    AddTextLine(strText, rec.Name);
                    AddTextLine(strText, string.Empty.PadLeft(rec.Name.Length, '-'));
                }
                else
                {
                    str = string.Format("{0} : {1}", rec.Name.PadLeft(max), rec.Value);
                    AddTextLine(strText, str);
                }
            }
        }

        public static int FormatMasaListText(GPMasaListResults mlist, StringBuilder str)
        {
            string stt;

            str.Remove(0, str.Length);
            str.AppendLine(GPAppHelper.CenterString(getSharedStrings()[39], 60));
            str.AppendLine();
            str.AppendLine();
            str.AppendLine(GPAppHelper.CenterString(mlist.m_location.getLocation(0).getFullName(), 60));
            str.AppendFormat(getSharedStrings()[41], mlist.vc_start, mlist.vc_end);
            str.AppendLine();
            str.AppendLine(string.Empty.PadRight(60,'='));
            str.AppendLine();

            int i;

            for (i = 0; i < mlist.n_countMasa; i++)
            {
                stt = string.Format("{0} {1}", GPMasa.GetName(mlist.arr[i].masa), mlist.arr[i].year);
                str.Append(stt.PadRight(30));
                stt = string.Format("{0} - ", mlist.arr[i].vc_start);
                str.Append(stt.PadLeft(16));
                stt = string.Format("{0}", mlist.arr[i].vc_end);
                str.Append(stt.PadLeft(13));
                str.AppendLine();
            }

            return 1;
        }

        public static void AvcGetTodayInfo(GPGregorianTime vc, GPLocationProvider loc, StringBuilder str)
        {
            string str2;

            GPCalendarResults db = new GPCalendarResults();

            GPGregorianTime vc2 = new GPGregorianTime(vc);
            vc2.AddDays(-4);
            db.CalculateCalendar(vc2, 9);

            int i = db.FindDate(vc);
            GPCalendarDay p = db.get(i);

            if (p == null)
                return;

            vc.InitWeekDay();

            str.AppendFormat("{0}, {1} {2}", loc.getFullName(), loc.getLocation(0).getLatitudeString(), loc.getLocation(0).getLongitudeString());
            str.AppendLine();
            str.AppendFormat("{0}: {1}", getSharedStrings()[12], loc.getLocation(0).getTimeZoneString());
            str.AppendLine();
            str.AppendLine();
            str.AppendFormat("[{0} - {1}]", vc, getSharedStrings()[vc.getDayOfWeek()]);
            str.AppendLine();
            str.AppendFormat("  {0}, {1} {2}", GPTithi.getName(p.astrodata.nTithi), GPPaksa.getName(p.astrodata.nPaksa), getSharedStrings()[20]);
            str.AppendLine();
            str.Append("  ");
            str.AppendFormat("{0}, {1}", p.getMasaLongName(), p.getGaurabdaYearLongString());
            str.AppendLine();
            str.AppendLine();

            if (p.hasEkadasiParana())
            {
                str.AppendLine(p.getEkadasiParanaString());
            }

            // adding mahadvadasi
            // adding spec festivals

            foreach (GPCalendarDay.Festival fest in p.CompleteFestivalList(db.get(i-1), db.get(i+1)))
            {
                if (GPUserDefaults.BoolForKey(fest.ShowSettingItem, true))
                {
                    if (fest.ShowSettingItem == GPDisplays.Keys.CalendarSankranti)
                    {
                        str.AppendLine(GPAppHelper.CenterString(fest.Text, 80, '-'));
                    }
                    else
                    {
                        str.AppendFormat("   {0}", fest.Text);
                        str.AppendLine();
                    }
                }
            }


            str.AppendLine();


            if (GPDisplays.Today.BrahmaMuhurtaVisible())
            {
                str.AppendLine();
                str.AppendFormat("{0} {1}", getSharedStrings()[988], p.astrodata.sun.arunodaya.getShortMuhurtaRange(0));
            }

            if (GPDisplays.Today.SunriseVisible())
            {
                str.AppendLine();
                str2 = string.Format("{0} {1} ",
                    getSharedStrings()[51], p.astrodata.sun.rise.getShortTimeString());
                str.Append(str2);
                if (GPDisplays.Today.SandhyaTimesVisible())
                {
                    str.AppendFormat(" {0} {1}", getSharedStrings()[989], p.astrodata.sun.rise.getShortSandhyaRange());
                }
                str.AppendFormat(" ({0})", GPAppHelper.GetDSTSignature(p.isDaylightInEffect()));
                str.AppendLine();
            }
            if (GPDisplays.Today.NoonVisible())
            {
                str2 = string.Format("{0}    {1} ", getSharedStrings()[857], p.astrodata.sun.noon.getShortTimeString());
                str.Append(str2);
                if (GPDisplays.Today.SandhyaTimesVisible())
                {
                    str.AppendFormat(" {0} {1} ", getSharedStrings()[989], p.astrodata.sun.noon.getShortSandhyaRange());
                }
                str2 = string.Format(" ({0})", GPAppHelper.GetDSTSignature(p.isDaylightInEffect()));
                str.Append(str2);
                str.AppendLine();
            }
            if (GPDisplays.Today.SunsetVisible())
            {
                str2 = string.Format("{0}  {1} ", getSharedStrings()[52], p.astrodata.sun.set.getShortTimeString());
                str.Append(str2);
                if (GPDisplays.Today.SandhyaTimesVisible())
                {
                    str.AppendFormat(" {0} {1} ", getSharedStrings()[989], p.astrodata.sun.set.getShortSandhyaRange());
                }
                str.AppendFormat(" ({0})", GPAppHelper.GetDSTSignature(p.isDaylightInEffect()));
                str.AppendLine();
            }

            if (GPDisplays.Today.SunriseInfo())
            {
                str.AppendLine();
                str.AppendLine(getSharedStrings()[990]);
                str.AppendLine();
                str.AppendFormat("   {0} {1}", GPNaksatra.getName(p.astrodata.nNaksatra), getSharedStrings()[15]);
                if (GPDisplays.Today.NaksatraPadaVisible())
                {
                    str.AppendFormat(", {0} {1} ({2})", p.getNaksatraElapsedString(), getSharedStrings()[993], getSharedStrings()[811 + p.getNaksatraPada()]);
                }
                if (GPDisplays.Today.RasiOfMoonVisible())
                {
                    str.AppendFormat(", {0}: {1}", getSharedStrings()[991], GPSankranti.getName(p.astrodata.nMoonRasi), getSharedStrings()[105]);
                }
                str.AppendFormat(", {0} {1}", GPYoga.getName(p.astrodata.nYoga), getSharedStrings()[104]);
                str.AppendLine();
                str.AppendFormat("   {0}: {1}.", getSharedStrings()[992], GPSankranti.getName(p.astrodata.nSunRasi));
                str.AppendLine();
            }

            AddNoteText(str);
        }
    }
}
