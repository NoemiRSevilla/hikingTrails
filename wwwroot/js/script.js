$(document).ready(function (){
    Console.WriteLine("Document is ready!");
    function initialize() {
        // Create a map centered in Pyrmont, Sydney (Australia).
        map = new google.maps.Map(document.getElementById('map'), 
            {
                center: { lat: -33.8666, lng: 151.1958 },
                zoom: 15
            }
        );
    }

    function callback(results, status) {
        if (status == google.maps.places.PlacesServiceStatus.OK) 
        {
            var marker = new google.maps.Marker(
                {
                    map: map,
                    place: 
                        {
                            placeId: results[0].place_id,
                            location: results[0].geometry.location
                        }
                }
            );
        }
    }

    google.maps.event.addDomListener(window, 'load', initialize);
});