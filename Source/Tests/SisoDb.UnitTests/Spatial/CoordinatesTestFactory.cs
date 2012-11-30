using System.Collections.Generic;
using SisoDb.Spatial;

namespace SisoDb.UnitTests.Spatial
{
    public static class CoordinatesTestFactory
    {
        public static Coordinates[] WrongSouthToNorthAndManyWestAndBackClosed()
        {
            var coordinates = new List<Coordinates>();
            coordinates.Add(new Coordinates { Latitude = 58.068356082112246, Longitude = 11.82043981552126 });
            coordinates.Add(new Coordinates { Latitude = 58.06866818143402, Longitude = 11.819613695144673 });
            coordinates.Add(new Coordinates { Latitude = 58.068549016560375, Longitude = 11.818626642227192 });
            coordinates.Add(new Coordinates { Latitude = 58.06822556704151, Longitude = 11.81819748878481 });
            coordinates.Add(new Coordinates { Latitude = 58.067675128668654, Longitude = 11.817489385604878 });
            coordinates.Add(new Coordinates { Latitude = 58.06739706894528, Longitude = 11.81919527053835 });
            coordinates.Add(new Coordinates { Latitude = 58.06806667848543, Longitude = 11.81920599937441 });
            coordinates.Add(new Coordinates { Latitude = 58.068356082112246, Longitude = 11.82043981552126 });
            return coordinates.ToArray();
        }

        public static Coordinates[] WrongOrderAndClosed()
        {
            var coordinates = new List<Coordinates>();
            coordinates.Add(new Coordinates { Latitude = 58.04836803946142, Longitude = 11.85287430196809 });
            coordinates.Add(new Coordinates { Latitude = 58.04804298770918, Longitude = 11.852927946148387 });
            coordinates.Add(new Coordinates { Latitude = 58.04817073765879, Longitude = 11.852665089664928 });
            coordinates.Add(new Coordinates { Latitude = 58.04836803946142, Longitude = 11.85287430196809 });
            return coordinates.ToArray();
        }

        public static Coordinates[] WrongOrderNotClosed()
        {
            var coordinates = new List<Coordinates>();
            coordinates.Add(new Coordinates { Latitude = 59.8297423369731, Longitude = 17.519133203852 });//4
            coordinates.Add(new Coordinates { Latitude = 59.8296936773323, Longitude = 17.5190071588812 });//3
            coordinates.Add(new Coordinates { Latitude = 59.8298203729009, Longitude = 17.5189979955459 });//2
            coordinates.Add(new Coordinates { Latitude = 59.8298223425544, Longitude = 17.5191345140461 });//1
            return coordinates.ToArray();
        }

        public static Coordinates[] CorrectOrderAndClosed()
        {
            var coordinates = new List<Coordinates>();
            coordinates.Add(new Coordinates { Latitude = 58.04852510452791, Longitude = 11.851250409710307 });
            coordinates.Add(new Coordinates { Latitude = 58.04842148618687, Longitude = 11.851124345886607 });
            coordinates.Add(new Coordinates { Latitude = 58.048364708886254, Longitude = 11.851494490730662 });
            coordinates.Add(new Coordinates { Latitude = 58.04852510452791, Longitude = 11.851250409710307 });
            return coordinates.ToArray();
        }

        public static Coordinates[] CorrectOrderNotClosed()
        {
            var coordinates = new List<Coordinates>();
            coordinates.Add(new Coordinates { Latitude = 58.04852510452791, Longitude = 11.851250409710307 });
            coordinates.Add(new Coordinates { Latitude = 58.04842148618687, Longitude = 11.851124345886607 });
            coordinates.Add(new Coordinates { Latitude = 58.048364708886254, Longitude = 11.851494490730662 });
            return coordinates.ToArray();
        }

        public static Coordinates[] CapeTown()
        {
            var coordinates = new List<Coordinates>();
            coordinates.Add(new Coordinates { Longitude = 18.623324871063232, Latitude = -33.87930689800001 });
            coordinates.Add(new Coordinates { Longitude = 18.518954753875732, Latitude = -33.814295035787055 });
            coordinates.Add(new Coordinates { Longitude = 18.469516277313232, Latitude = -33.90666550953439 });
            coordinates.Add(new Coordinates { Longitude = 18.410464763641357, Latitude = -33.90438596045654 });
            coordinates.Add(new Coordinates { Longitude = 18.365146160125732, Latitude = -33.929457647348386 });
            coordinates.Add(new Coordinates { Longitude = 18.366519451141357, Latitude = -33.97046812793053 });
            coordinates.Add(new Coordinates { Longitude = 18.443423748016357, Latitude = -34.00804370312774 });
            coordinates.Add(new Coordinates { Longitude = 18.446170330047607, Latitude = -34.07290781948443 });
            coordinates.Add(new Coordinates { Longitude = 18.466769695281982, Latitude = -34.10361560806379 });
            coordinates.Add(new Coordinates { Longitude = 18.638431072235107, Latitude = -34.0694951555098 });
            coordinates.Add(new Coordinates { Longitude = 18.693362712860107, Latitude = -34.0694951555098 });
            coordinates.Add(new Coordinates { Longitude = 18.731814861297607, Latitude = -34.04332683182753 });
            coordinates.Add(new Coordinates { Longitude = 18.741427898406982, Latitude = -33.97844009281468 });
            coordinates.Add(new Coordinates { Longitude = 18.700229167938232, Latitude = -33.91578309625544 });
            coordinates.Add(new Coordinates { Longitude = 18.679629802703857, Latitude = -33.880447015354235 });
            coordinates.Add(new Coordinates { Longitude = 18.623324871063232, Latitude = -33.87930689800001 });
            return coordinates.ToArray();
        }

        public static Coordinates[] Castle()
        {
            var coordinates = new List<Coordinates>();
            coordinates.Add(new Coordinates { Longitude = 9.520484685897834, Latitude = 49.758400165268135 });
            coordinates.Add(new Coordinates { Longitude = 9.520565152168281, Latitude = 49.75866354094329 });
            coordinates.Add(new Coordinates { Longitude = 9.520715355873115, Latitude = 49.75882641728967 });
            coordinates.Add(new Coordinates { Longitude = 9.520715355873115, Latitude = 49.758954638709376 });
            coordinates.Add(new Coordinates { Longitude = 9.520940661430366, Latitude = 49.759068998067924 });
            coordinates.Add(new Coordinates { Longitude = 9.520865559577949, Latitude = 49.75917296088711 });
            coordinates.Add(new Coordinates { Longitude = 9.520688533782966, Latitude = 49.759211080531635 });
            coordinates.Add(new Coordinates { Longitude = 9.520618796348579, Latitude = 49.759328904697945 });
            coordinates.Add(new Coordinates { Longitude = 9.520382761955268, Latitude = 49.75931504304618 });
            coordinates.Add(new Coordinates { Longitude = 9.520173549652107, Latitude = 49.75924920014618 });
            coordinates.Add(new Coordinates { Longitude = 9.519894599914558, Latitude = 49.75923533847163 });
            coordinates.Add(new Coordinates { Longitude = 9.51957273483277, Latitude = 49.759335835522336 });
            coordinates.Add(new Coordinates { Longitude = 9.51936352252961, Latitude = 49.75936702421987 });
            coordinates.Add(new Coordinates { Longitude = 9.51910603046418, Latitude = 49.75913484121263 });
            coordinates.Add(new Coordinates { Longitude = 9.519030928611762, Latitude = 49.75893384606976 });
            coordinates.Add(new Coordinates { Longitude = 9.519127488136299, Latitude = 49.758760573726164 });
            coordinates.Add(new Coordinates { Longitude = 9.519288420677192, Latitude = 49.75860462808753 });
            coordinates.Add(new Coordinates { Longitude = 9.51936352252961, Latitude = 49.758424423613825 });
            coordinates.Add(new Coordinates { Longitude = 9.519481539726264, Latitude = 49.75831006273507 });
            coordinates.Add(new Coordinates { Longitude = 9.519781947135932, Latitude = 49.75825114944985 });
            coordinates.Add(new Coordinates { Longitude = 9.520484685897834, Latitude = 49.758400165268135 });
            return coordinates.ToArray();
        }

        public static Coordinates[] StartThatDoesNotRenderWithoutFix()
        {
            var coordinates = new List<Coordinates>();
            coordinates.Add(new Coordinates { Longitude = 14.413786888122559, Latitude = 35.91277231730788 });
            coordinates.Add(new Coordinates { Longitude = 14.423571586608887, Latitude = 35.93334614363321 });
            coordinates.Add(new Coordinates { Longitude = 14.435416221618652, Latitude = 35.913467466361155 });
            coordinates.Add(new Coordinates { Longitude = 14.454127311706543, Latitude = 35.91374552427219 });
            coordinates.Add(new Coordinates { Longitude = 14.437304496765137, Latitude = 35.90151005169695 });
            coordinates.Add(new Coordinates { Longitude = 14.452582359313965, Latitude = 35.89233220575431 });
            coordinates.Add(new Coordinates { Longitude = 14.424944877624512, Latitude = 35.90192720122905 });
            coordinates.Add(new Coordinates { Longitude = 14.401598930358887, Latitude = 35.894696298117694 });
            coordinates.Add(new Coordinates { Longitude = 14.408808708190918, Latitude = 35.9052643183337 });
            coordinates.Add(new Coordinates { Longitude = 14.394217491149902, Latitude = 35.912911347607185 });
            coordinates.Add(new Coordinates { Longitude = 14.413786888122559, Latitude = 35.91277231730788 });
            return coordinates.ToArray();
        }
    }
}