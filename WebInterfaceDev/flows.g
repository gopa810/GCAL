
# definitions of pages

# this is main menu
page mainmenu
  source mainmenu
  action calc
    goto mainmenu-calc
  end action
  action set
    goto mainmenu-set
  end action
  action geo
    goto mainmenu-geo
  end action
  action help
    goto help
  end action
  action search
    goto dlg-find
  end action
end page

# help page

page help
  source help
end page

# this is menu for calculation

page mainmenu-calc
  source mainmenu-calc
  action today
    goto today
  end action
  action nextfest
    goto nextfest
  end action
  action search
    goto dlg-find
  end action
  action calendar
    set $nextpage dlg-startdate-cal
    goto dlg-enterloc
  end action
  action coreevents
    set $nextpage dlg-startdate-core
    goto dlg-enterloc
  end action
  action appday
    set $nextpage dlg-startdate-app
    goto dlg-enterloc
  end action
  action masalist
    set $nextpage dlg-startyear
    goto dlg-enterloc
  end action
  action cal2cal
    goto dlg-enterloc-a
  end action
  action cal2core
    set $nextpage dlg-startdate-cal2core
    goto dlg-enterloc
  end action
end page

page today
  source today
  button top "$372" 'action:settings'
  button bottom "< $369" 'today:prev'
  button bottom "$43" 'today:today'
  button bottom "$370 >" 'today:next'
  action settings
    goto set-disp-today
  end action
end page

page nextfest
  source nextfest
  button top "$372" 'action:settings'
  action settings
    goto set-disp-nextfest
  end action
end page

page dlg-find
  source dlg-find
end page

page dlg-enterloc
  source dlg-enterloc
  set $currTitle $1033
  action mylocation
    set $locationtype mylocation
    goto $nextpage
  end action
  action full
    set $locationtype entered
    goto dlg-locfull
  end action
  action select
    set $locationtype selected
    goto dlg-selloc
  end action
end page

page dlg-enterloc-a
  source dlg-enterloc
  set $currTitle $308
  set $ppx 'a'
  set $nextpage dlg-enterloc-b
  action mylocation
    set $locationtype mylocation
    goto $nextpage
  end action
  action full
    set $locationtype entered
    goto dlg-locfull
  end action
  action select
    set $locationtype selected
    goto dlg-selloc
  end action
end page

page dlg-enterloc-b
  source dlg-enterloc
  set $currTitle $309
  set $ppx 'b'
  set $nextpage dlg-startdate-cal2cal
  action mylocation
    set $locationtypeb mylocation
    goto $nextpage
  end action
  action full
    set $locationtypeb entered
    goto dlg-locfull
  end action
  action select
    set $locationtypeb selected
    goto dlg-selloc
  end action
end page

page dlg-selloc
  source dlg-selloc
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    goto $nextpage
  end action
end page

page dlg-locfull
  source dlg-locfull
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    goto dlg-country
  end action
end page

page dlg-country
  source dlg-country
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    goto dlg-enterlongitude
  end action
end page

page dlg-enterlongitude
  source dlg-enterlongitude
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    goto dlg-enterlatitude
  end action
end page

page dlg-enterlatitude
  source dlg-enterlatitude
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    goto dlg-selcoutz
  end action
end page

page dlg-selcoutz
  source dlg-selcoutz
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    goto $nextpage
  end action
end page

page dlg-startdate-cal
  source dlg-startdate
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    set $nextpage calendar
    goto dlg-endperiod-x
  end action
end page

page dlg-startdate-core
  source dlg-startdate
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    set $nextpage coreevents
    goto dlg-endperiod-x
  end action
end page

page dlg-startdate-app
  source dlg-startdate
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    goto dlg-time-app
  end action
end page

page dlg-startdate-cal2cal
  source dlg-startdate
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    set $nextpage cal2locs
    goto dlg-endperiod-x
  end action
end page

page dlg-startdate-cal2core
  source dlg-startdate
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    set $nextpage calcore
    goto dlg-endperiod-x
  end action
end page

page dlg-endperiod-x
  source dlg-endperiod
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    goto $nextpage
  end
end page

page dlg-startyear
  source dlg-startyear
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    goto dlg-yearcount
  end action
end page

page dlg-yearcount
  source dlg-yearcount
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    goto masalist
  end action
end page

page dlg-time-app
  source dlg-time
  button bottom "$239 >" 'action:next'
  action next
    script "onSave"
    goto appday
  end action
end page

page calendar
  source calendar
  button top $1062 saveContent
  button top "$372" 'action:settings'
  action settings
    goto set-disp-cal
  end action
end page

page calcore
  button top $1062 saveContent
  source calcore
end page

page cal2locs
  button top $1062 saveContent
  source cal2locs
  button top "$372" 'action:settings'
  action settings
    goto set-disp-cal
  end action
end page

page masalist
  button top $1062 saveContent
  source masalist
  button top "$372" 'action:settings'
  action settings
    goto set-disp-masa
  end action
end page

page coreevents
  button top $1062 saveContent
  source coreevents
  button top "$372" 'action:settings'
  action settings
    goto set-disp-core
  end action
end page

page appday
  button top $1062 saveContent
  source appday
  button top "$372" 'action:settings'
  action settings
    goto set-disp-app
  end action
end page

page mainmenu-set
  source mainmenu-set
  action disp
    goto set-disp
  end action
  action general
    goto set-general
  end action
  action startpage
    goto set-startpage
  end action
  action mylocation
    set $nextpage mainmenu-set
	goto dlg-enterloc
  end action
  action languages
    goto languages
  end action
end page

page set-disp
  source set-disp
  action cal
    goto set-disp-cal
  end action
  action core
    goto set-disp-core
  end action
  action app
    goto set-disp-app
  end action
  action masa
    goto set-disp-masa
  end action
  action today
    goto set-disp-today
  end action
  action nextfest
    goto set-disp-nextfest
  end action
end page

page set-disp-cal
  source set-disp-cal
  button bottom "$44" "action:calendar"
  action ann
    script "onSave"
    goto set-disp-cal-ann
  end action
  action hdt
    script "onSave"
    goto set-disp-cal-hdt
  end action
  action set-disp
    goto set-disp
  end action
  action calendar
    script "onSave"
    goto calendar
  end action
end page

page set-disp-cal-ann
  source set-disp-cal-ann
  button bottom "$44" "action:calendar"
  action set-disp-cal
    script "onSave"
    goto set-disp-cal
  end action
  action calendar
    script "onSave"
    goto calendar
  end action
end page

page set-disp-cal-hdt
  source set-disp-cal-hdt
  button bottom "$44" "action:calendar"
  action set-disp-cal
    goto set-disp-cal
  end action
  action calendar
    script "onSave"
    goto calendar
  end action
end page

page set-disp-core
  source set-disp-core
  button bottom "$46" "action:coreevents"
  action sort
    goto set-disp-core-sort
  end action
  action coreevents
    script "onSave"
    goto coreevents
  end action
end page


page set-disp-core-sort
  source set-disp-core-sort
  button bottom "$46" "action:coreevents"
  action set-disp-core
   goto set-disp-core
  end action
  action coreevents
    script "onSave"
    goto coreevents
  end action
end page

page set-disp-app
  source set-disp-app
  button bottom "$45" "action:calculate"
  action calculate
    goto appday
  end action
end page

page set-disp-masa
  source set-disp-masa
  button bottom "$48" "action:calculate"
  action calculate
    goto masalist
  end action
end page

page set-disp-today
  source set-disp-today
  button bottom "$43" "action:calculate"
  action calculate
    goto today
  end action
end page


page set-disp-nextfest
  source set-disp-nextfest
  button bottom "$452" "action:calculate"
  action calculate
    goto nextfest
  end action
end page

page set-startpage
  source set-startpage
end page

page set-general
  source set-general
  action catur
    goto set-gen-catur
  end action
  action fdow
    goto set-gen-fdow
  end action
  action mnf
    goto set-gen-mnf
  end action
  action fast
    goto set-gen-fast
  end action
  action tformat
    goto set-gen-tformat
  end action
  action snf
    goto set-gen-snf
  end action
end page


page set-gen-catur
  source set-gen-catur
end page

page set-gen-fdow
  source set-gen-fdow
end page

page set-gen-mnf
  source set-gen-mnf
end page

page set-gen-fast
  source set-gen-fast
end page

page set-gen-tformat
  source set-gen-tformat
end page

page set-gen-snf
  source set-gen-snf
end page

page languages
  source languages
end page

page mainmenu-geo
  source mainmenu-geo
  action cities
    goto geo-cities
  end action
  action countries
    goto geo-countries
  end action
  action events
    goto geo-events
  end action
  action tzones
    goto geo-tzones
  end action
end page

page geo-cities
  source geo-cities
  action add
    set $ppx ""
    set $locationtype entered
    set $nextpage dlg-savelocation
    set $currTitle $1044
    goto dlg-locfull-c
  end action
  action edit
    set $ppx ""
    set $locationtype selected
    set $currTitle $249
    set $nextpage dlg-editlocation-ce
    goto dlg-selloc-x
  end action
  action remove
    set $nextpage dlg-dellocation-ce
    goto dlg-selloc-x
  end action
end page

page dlg-selloc-x
  source dlg-selloc
  button bottom "$239 >" 'action:next'
  action next
    exec loadlocationid
    goto $nextpage
  end action
end page

page dlg-locfull-c
  source dlg-locfull
  button bottom "$239 >" 'action:next'
  action next
    goto dlg-country-c
  end action
end page

page dlg-country-c
  source dlg-country
  button bottom "$239 >" 'action:next'
  action next
    goto dlg-enterlongitude-c
  end action
end page

page dlg-enterlongitude-c
  source dlg-enterlongitude
  button bottom "$239 >" 'action:next'
  action next
    goto dlg-enterlatitude-c
  end action
end page

page dlg-enterlatitude-c
  source dlg-enterlatitude
  button bottom "$239 >" 'action:next'
  action next
    goto dlg-selcoutz-c
  end action
end page

page dlg-selcoutz-c
  source dlg-selcoutz
  button bottom "$239 >" 'action:next'
  action next
    goto dlg-savelocation-c
  end action
end page

page dlg-savelocation-c
  source dlg-savelocation
  action save
  end action
end page

page dlg-editlocation-ce
  source dlg-editlocation
  action save
  end action
end page

page dlg-dellocation-ce
  source dlg-dellocation
  action delete
  end action
end page

page geo-countries
  source geo-countries
  action add
    goto dlg-newcountry
  end action
  action edit
    goto dlg-country-coe
  end action
  action delete
	goto dlg-country-cod
  end action
end page

page dlg-country-coe
  source dlg-country
  action next
    goto dlg-renamecountry
  end action
end page

page dlg-country-cod
  source dlg-country
  action next
    goto dlg-confirm-del-country
  end action
end page

page dlg-newcountry
  source dlg-newcountry
  action next
    goto dlg-editcoutz
  end action
end page

page dlg-editcoutz
  source dlg-editcoutz
end page

page dlg-renamecountry
  source dlg-renamecountry
end page

page dlg-confirm-del-country
  source dlg-confirm-del-country
end page

page geo-events
  source geo-events
  action add
    exec initnewevent
    set $currTitle $1164
    goto dlg-editevent-add
  end action
  action edit
    goto dlg-findevent-ee
  end action
  action delete
    goto dlg-findevent-ed
  end action
end page

page dlg-findevent-ee
  source dlg-findevent
  action next
    set $disableeventtype 1
    exec loadeventid
    set $currTitle $1165
    goto dlg-editevent-edit
  end action
end page

page dlg-findevent-ed
  source dlg-findevent
  action next
    exec loadeventid
    goto dlg-delevent
  end action
end page

page dlg-editevent-add
  source dlg-editevent
  action next
    exec newevent
    goto geo-events
  end action
end page

page dlg-editevent-edit
  source dlg-editevent
  action next
    exec savechangedevent
    set $disableeventtype 0
    goto geo-events
  end action
end page

page dlg-delevent
  source dlg-delevent
  action next
    exec removeeventid
    goto geo-events
  end action
end page

page geo-tzones
  source geo-tzones
  action add
    set $tzdata ""
    set $nextedittzaction "savetzone"
    set $currTitle $349
    goto dlg-edittimezone
  end action
  action edit
    set $tzdata ""
    set $nextedittzaction "updatetzone"
    set $currTitle $1097
    set $nextTitle $1120
    goto dlg-findtz-te
  end action
  action delete
    set $tzdata ""
    set $nextTitle $343
    goto dlg-findtz-td
  end action
end page

page dlg-edittimezone
  source dlg-edittimezone
  action next
    exec $nextedittzaction
    goto geo-tzones
  end action
end page

page dlg-findtz-te
  source dlg-findtz
  action next
    exec loadtzone
    goto dlg-edittimezone
  end action
end page

page dlg-findtz-td
  source dlg-findtz
  action next
    exec loadtzone
    goto dlg-deltzone
  end action
end page

page dlg-deltzone
  source dlg-deltzone
  action next
    exec deltzone
    goto geo-tzones
  end action
  action cancel
    goto geo-tzones
  end action
end page

