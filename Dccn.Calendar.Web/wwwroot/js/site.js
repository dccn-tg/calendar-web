"use strict";

jQuery(function($) {
    $(".calendar").each(function() {
        var $calendar = $(this);
        $calendar.fullCalendar({
            header: {
                left: "prev,next today",
                center: "title",
                right: "month,agendaWeek,agendaDay"
            },
            locale: window.navigator.userLanguage || window.navigator.language || "en",
            minTime: "08:00:00",
            maxTime: "20:00:00",
            defaultView: "agendaWeek",
            navLinks: true,
            eventLimit: true,
            timezone: "local",
            weekends: false,
            eventColor: "#BE311A",
            eventTextColor: "white",
            eventSources: [
                $calendar.attr("data-calendar-api-url")
            ],
            eventDataTransform: function(event) {
                if (event.recurring) {
                    event.color = "#8E0000";
                }
                return event;
            },
            eventRender: function(event, $elem) {
                var text;
                if (event.allDay) {
                    text = event.start.format("L") + " - " + event.end.format("L");
                } else {
                    text = event.start.format("LT") + " - " + event.end.format("LT");
                }
                if (event.recurring) {
                    text += " (recurring)";
                }
                $elem.qtip({
                    content: {
                        title: event.title,
                        text: text
                    },
                    style: {
                        classes: "qtip-dark qtip-rounded"
                    },
                    position: {
                        target: "mouse",
                        adjust: {
                            mouse: false
                        }
                    },
                    show: {
                        solo: true
                    },
                    hide: {
                        event: "click mouseleave"
                    }
                });
            },
            loading: function(isLoading) {
                if (isLoading) {
                    $calendar.LoadingOverlay("show", {
                        fade: [200, 200],
                        size: 10
                    });
                } else {
                    $calendar.LoadingOverlay("hide");
                }
            }
        });
    });
});