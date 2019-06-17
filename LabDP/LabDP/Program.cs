/*
 * Puntos del ejercicio: 13 + 2 extras
 *
 * Acabas de fundar un nuevo start-up llamado hUBER para proveer servicio de
 * taxi usando carros autonomos (self-driving cars). Como esos carros son caros
 * y tu empresa aun es joven, decides comprar solamente dos vehiculos.
 *
 * Como el trafico en la ciudad es caotica y las autoridades no confian en el
 * funcionamiento de esos vehiculos autonomos, tu licencia te limita a operar
 * en una de las avenidas lejos del eje central de la ciudad. Esa avenida es
 * bidireccional, empieza en el kilometro 0 y se extiende en una sola dimension
 * hasta el infinito.  Tus dos carros comienzan el dia estacionados en el garage
 * de tu empresa, el cual se localiza en el kilometro 'headquarterPos'.
 *
 * El tiempo que toma un vehiculo de llegar de el kilometro X al kilometro Y
 * es la cantidad de kilometros entre las dos posiciones.  Es decir, asume que
 * la velocidad de tus vehiculos es constante de 1 Km por minuto y aceleran de
 * manera instantanea!. Matematicamente, la distancia entre las posiciones X y Y
 * es |X - Y| (donde la barra indica valor absoluto).
 *
 * Para cada viaje, calculas la tarifa de la siguiente manera:
 *   1) Tienes un cargo fijo de 'flatFee' pesos
 *   2) Los primeros 'flatFeeThreshold' kilometros del viaje estan cubiertos por
 *      ese cargo fijo: si tu ride no excede 'flatFeeThreshold' kilometros, el
 *      cliente solamente paga el flat fee.
 *   3) A partir de ahi, el costo es de 'costPerAdditionalKm' por kilometro
 *      adicional recorrido
 *   4) El "millero" empieza a contar a partir de que el pasajero se monte en el
 *      vehiculo y se detiene cuando el pasajero de desmonte en su destino
 *
 * Al final del dia, revisas la bitacora de todas las solicitudes de servicio
 * que recibiste durante el dia.  Cada solicitud tiene registrado los siguientes
 * campos (ver la clase RideRequest):
 *   # 'time': la hora en que el cliente solicito el servicio, medido en minutos
 *             desde el inicio del dia. Puedes asumir que la bitacora esta
 *             ordenada cronologicamente por este campo
 *   # 'start': posicion donde debes recoger el pasajero, medido en unidades de
 *              kilometros desde el kilometro 0
 *   # 'end': posicion destino del viaje, tambien medido desde el kilometro 0
 *
 * Basado en esa bitacora, tu quieres determinar cual es la maxima ganancia que
 * pudiste haber obtenido, siguiendo las siguientes reglas:
 *   a) Tienes la opcion de servir o ignorar cada solicitud de servicio
 *   b) Cada cliente es impaciente y no esta dispuesto a esperar ningun minuto:
 *      si el vehiculo llega a recogerlo 1 minuto despues de haber solicitado
 *      el taxi, ese cliente rechaza el servicio. Si llegas antes o justamente
 *      en el minuto que solicito el servicio, el cliente acepta el servicio
 *   c) Asume que el tiempo que el pasajero se toma para montar y desmontarse
 *      del vehiculo es instantaneo (0 minutos)
 *   d) No se permite desmontar el pasajero antes de llegar a su destino!
 *   e) No se permite ride sharing (varios pasajeros compartiendo el mismo
 *      viaje)
 *   f) Es obvio que mas de un vehiculo no puede aceptar la misma solicitud
 *   g) Una vez que llega al destino y el pasajero se desmonta, el vehiculo se
 *      puede quedar estacionado en el mismo punto destino o ir a otra posicion
 *      (a la velocidad de 1 km/min)
 *
 * Ubica todas las secciones con el tag TODO.  Cada tag les indicara que deben
 * completar.  Para TODOs que requieren respuestas, escribe las respuestas en
 * un comment.
 *
 * Puedes agregar todos los metodos y atributos que desees, pero no debes
 * alterar la firma del metodo Solve.
 * 
 */


using System;
using System.Text;
using System.Collections.Generic;
using System.IO;


// Definicion de una solicitud de taxi
public class RideRequest
{
    public int id;
    public int time;        // hora en minutos en que pidieron este ride
    public int start, end;  // posicion de origen y destino del ride
}


// Esta clase abstract define el metodo Solve que debes implementar.  Esto es
// para facilitar la correccion de esta tarea

public abstract class ATaxiRideService
{
    // OJO: tu solucion debe llenar estas dos propiedades:

    public long MaxEarning { get; protected set; } // ganancia optima

    public List<RideRequest>[] AssignedRides { get; protected set; }
    // AssignedRides[0] = lista de solicitudes asignados al vehiculo 0
    // AssignedRides[1] = lista de solicitudes asignados al vehiculo 1


    // Este es el metodo que debes implementar
    public abstract void Solve(
        int headquarterPos,
        int flatFee,
        int flatFeeThreshold,
        int costPerAdditionalKm,
        RideRequest[] requests
    );
}


public class MyTaxiRideService : ATaxiRideService
{
    // Propiedades del servicio de taxis para usarlos de manera global en la clase
    private int hqPos { get; set; }
    private int flatFee { get; set; }
    private int flatTreshHold { get; set; }
    private int costPerKm { get; set; }
    private RideRequest[] rides { get; set; }

    // Propiedades de Dynamic Programming
    bool[,,] cached;
    long[,,] memo;
    int[,,] p; 

    public override void Solve(
        int headquarterPos,
        int flatFee,
        int flatFeeThreshold,
        int costPerAdditionalKm,
        RideRequest[] requests
    )
    {
        int posActual1 = headquarterPos;
        int posActual2 = headquarterPos;
        hqPos = headquarterPos;
        this.flatFee = flatFee;
        this.flatTreshHold = flatFeeThreshold;
        costPerKm = costPerAdditionalKm;
        rides = new RideRequest[requests.Length + 1];
        rides[0] = new RideRequest() { id = 0, time = 0, start = hqPos, end = hqPos };
        for (int i = 0, j = 1; i < requests.Length; i++, j++)
            rides[j] = requests[i];
        
        // TODO: implementa un algoritmo basado en Dynamic Programming para
        //       calcular la maxima ganancia de hUBER segun la bitacora de
        //       solicitudes grabados en el arreglo 'requests'.  Llena la
        //       propiedad MaxEarnings
        // Complejidad esperada: no exponencial
        // Valor: 8 puntos
        //        +2 puntos extras si tu algoritmo NO es pseudopolinomial
        int num = BuscarMayorNumero(requests);

        cached = new bool[requests.Length, requests.Length, requests.Length];
        memo = new long[requests.Length, requests.Length, requests.Length];
        p = new int[rides.Length, rides.Length, rides.Length];

        long res = F(rides, 1, 0, 0);
        this.MaxEarning = res;

        // TODO: describe la estructura de los subproblemas en tu solucion con
        //       Dynamic Programming: cuales son los parametros de tu DP y que
        //       significan?
        // Valor: 1 punto

        /*
         * Los parametros que utilice son el viaje actual que se esta calculando (n),
         * el ultimo viaje tomado por el vehiculo 1 (ultimoViaje1) y el ultimo viaje tomado por
         * el vehiculo 2 (ultimoViaje2). Los ultimos viajes de ambos vehiculos son para  
         * saber si el vehiculo termino ese ultimo viaje o esta en curso al momento del tiempo en que 
         * se necesite realizar el viaje n.
        */

        // TODO: modifica tu algoritmo para que tambien recupere la asignacion
        //       optima de solicitudes a vehiculo.  Llena la propiedad 
        //       AssignedRides
        // Valor: 3 puntos
        AssignedRides = new List<RideRequest>[]
        {
            new List<RideRequest>(),
            new List<RideRequest>()
        };
        int ultimoviaje1 = 0;
        int ultimoviaje2 = 0;
        for (int n = 1; n < rides.Length; n++)
        {
            if (p[n, ultimoviaje1, ultimoviaje2] == 1)
            {
                AssignedRides[0].Add(rides[n]);
                ultimoviaje1 = n;
            }
            else if (p[n, ultimoviaje1, ultimoviaje2] == 2)
            {
                AssignedRides[1].Add(rides[n]);
                ultimoviaje2 = n;
            }
        }

        // TODO: determina la complejidad de tu algoritmo
        // Valor: 1 punto
        // O(n^3)

    }

    

    //F(n, v) : Maxima ganancia que se pueden obtener de manera optima
    //          con dos vehiculos considerando los viajes desde la posicion
    //          n en adelante y que alguno de los vehiculos puede llegar a
    //          tiempo a realizar el viaje.
    public long F(RideRequest[] rides, int n, int ultimoViaje1, int ultimoViaje2)
    {
        if (n == rides.Length - 1)
            return 0;

        if (cached[n, ultimoViaje1, ultimoViaje2])
            return memo[n, ultimoViaje1, ultimoViaje2];

        RideRequest ride = rides[n];

        long res1 = 0, res2 = 0, res3 = 0;
        
        int tiempo1 = rides[ultimoViaje1].time + Math.Abs(rides[ultimoViaje1].end - rides[ultimoViaje1].start);
        int pos1 = rides[ultimoViaje1].end;
        int tiempo2 = rides[ultimoViaje2].time + Math.Abs(rides[ultimoViaje2].end - rides[ultimoViaje2].start);
        int pos2 = rides[ultimoViaje2].end;
        
        long profit = 0;

        if (PuedoLlegar(pos1, tiempo1, ride.start, ride.time))
        {
            profit = GananciaDelViaje(ride.end, ride.start);
            res1 = F(rides, n + 1, n, ultimoViaje2) + profit;
        }
        else if (PuedoLlegar(pos2, tiempo2, ride.start, ride.time))
        {
            profit = GananciaDelViaje(ride.end, ride.start);
            res2 = F(rides, n + 1, ultimoViaje1, n) + profit;
        }

        res3 = F(rides, n + 1, ultimoViaje1, ultimoViaje2);

        long res = Max(res1, res2, res3);
        if (res == res1)
            p[n, ultimoViaje1, ultimoViaje2] = 1;
        if (res == res2)
            p[n, ultimoViaje1, ultimoViaje2] = 2;
        if (res == res3)
            p[n, ultimoViaje1, ultimoViaje2] = 3;


        cached[n, ultimoViaje1, ultimoViaje2] = true;
        memo[n, ultimoViaje1, ultimoViaje2] = res;

        return res;
    }


    private int TiempoActual(int tiempoInicio, int posFinal, int posInicial)
    {
        return Math.Abs(posFinal - posInicial) + tiempoInicio;
    }

    private int Distancia(int posFinal, int posInicial)
    {
        return Math.Abs(posFinal - posInicial);
    }

    private bool PuedoLlegar(int posCarro, int tiempoActual, int posPasajero, int tiempoPasajero)
    {
        int distancia = Math.Abs(posCarro - posPasajero);
        if (distancia + tiempoActual > tiempoPasajero)
            return false;
        else
            return true;
    }

    private static long Max(long x, long y, long z)
    {
        if (x > y)
            return ((x > z) ? x : z);
        else
            return ((y > z) ? y : z);
    }

    private static int BuscarMayorNumero(RideRequest[] requests)
    {
        int mayor = int.MinValue;
        foreach(var r in requests)
        {
            if (r.start > mayor)
                mayor = r.start;
            if (r.end > mayor)
                mayor = r.end;
        }
        return mayor;
    }

    private long GananciaDelViaje(int posFinal, int posInicial)
    {
        int distancia = Math.Abs(posFinal - posInicial);
        return distancia > flatTreshHold ? (distancia - flatTreshHold) * costPerKm + flatFee : flatFee;
    }
}

class Lab3u
{

    public static void Main(string[] args)
    {

        //RideRequest[] requests =
        //{
        //    new RideRequest() { id =  1, time =  75, start =  5, end =  9 },
        //    new RideRequest() { id =  2, time =  90, start = 30, end = 18 },
        //    new RideRequest() { id =  3, time = 100, start = 15, end = 25 },
        //    new RideRequest() { id =  4, time = 105, start = 44, end = 37 },
        //    new RideRequest() { id =  5, time = 120, start =  8, end = 18 },
        //    new RideRequest() { id =  6, time = 132, start =  7, end = 29 },
        //    new RideRequest() { id =  7, time = 140, start = 12, end = 18 },
        //    new RideRequest() { id =  8, time = 150, start = 52, end = 31 },
        //    new RideRequest() { id =  9, time = 159, start = 48, end = 23 },
        //    new RideRequest() { id = 10, time = 177, start = 25, end = 10 },
        //    new RideRequest() { id = 11, time = 190, start = 55, end = 60 },
        //    new RideRequest() { id = 12, time = 200, start = 50, end = 20 },
        //    new RideRequest() { id = 13, time = 209, start = 25, end = 10 },
        //    new RideRequest() { id = 14, time = 215, start = 10, end =  7 },
        //    new RideRequest() { id = 15, time = 224, start = 60, end = 40 },
        //    new RideRequest() { id = 16, time = 260, start = 42, end = 13 },
        //    new RideRequest() { id = 17, time = 260, start = 42, end = 13 },
        //    new RideRequest() { id = 18, time = 280, start =  1, end = 25 },
        //    new RideRequest() { id = 19, time = 300, start = 32, end = 21 },
        //    new RideRequest() { id = 20, time = 300, start = 32, end = 21 },
        //};

        //int headquarterPos = 10;
        //int flatFee = 50;
        //int flatFeeThreshold = 5;
        //int costPerAdditionalKm = 6;


        //Para Windows, cambia este nombre por el full path del fichero que
        //contiene el test case.  Ejemplo: @"C:\Users\JohnDoe\input1.txt"
        string filename = @"C:\Users\Erick\Source\Repos\LabDP\TestCasesDP\input5.txt";

        int headquarterPos, flatFee, flatFeeThreshold, costPerAdditionalKm;
        RideRequest[] requests;

        ReadTestFromFile(filename,
                         out headquarterPos,
                         out flatFee,
                         out flatFeeThreshold,
                         out costPerAdditionalKm,
                         out requests);

        ATaxiRideService s = new MyTaxiRideService();
        Console.WriteLine("Cantidad de requests: {0}", requests.Length);
        DateTime start = DateTime.Now;
        s.Solve(headquarterPos, flatFee, flatFeeThreshold, costPerAdditionalKm,
                requests);
        DateTime end = DateTime.Now;
        Console.WriteLine("Tiempo de ejecucion: {0} ms", (end - start).Milliseconds);
        Console.WriteLine("Ganancia optima es ${0}", s.MaxEarning);

        if (s.AssignedRides != null)
        {
            for (int j = 0; j < s.AssignedRides.Length; j++)
            {
                Console.Write("Solicitudes asignadas a vehiculo {0}: ", j);
                foreach (RideRequest r in s.AssignedRides[j])
                {
                    Console.Write(" {0}", r.id);
                }
                Console.WriteLine();
            }
        }
        Console.Read();
    }


    static void ReadTestFromFile(string filename,
                                 out int headquarterPos,
                                 out int flatFee,
                                 out int flatFeeThreshold,
                                 out int costPerAdditionalKm,
                                 out RideRequest[] requests)
    {
        headquarterPos = 0;
        flatFee = 0;
        flatFeeThreshold = 0;
        costPerAdditionalKm = 0;
        requests = new RideRequest[0];

        var fileStream = new FileStream(filename,
                                        FileMode.Open, FileAccess.Read);
        using (var streamReader = new StreamReader(fileStream))
        {
            char[] blanks = { ' ' };
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line == null)
                    throw new Exception("Input file ended prematurely");
                line = line.Trim();
                if (line.Length == 0 || line[0] == '#')
                    continue;
                string[] tokens
                    = line.Split(blanks, 4,
                                 StringSplitOptions.RemoveEmptyEntries);
                headquarterPos = (int)uint.Parse(tokens[0]);
                flatFee = (int)uint.Parse(tokens[1]);
                flatFeeThreshold = (int)uint.Parse(tokens[2]);
                costPerAdditionalKm = (int)uint.Parse(tokens[3]);
                break;
            }

            List<RideRequest> requestList = new List<RideRequest>();
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line == null)
                    throw new Exception("Input file ended prematurely");
                line = line.Trim();
                if (line.Length == 0 || line[0] == '#')
                    continue;
                string[] tokens
                    = line.Split(blanks, 3,
                                 StringSplitOptions.RemoveEmptyEntries);
                RideRequest r = new RideRequest()
                {
                    id = requestList.Count + 1,
                    time = (int)uint.Parse(tokens[0]),
                    start = (int)uint.Parse(tokens[1]),
                    end = (int)uint.Parse(tokens[2])
                };
                if (requestList.Count > 0)
                    if (requestList[requestList.Count - 1].time > r.time)
                        throw new Exception("Bitacora no esta ordenada cronologicamente");
                requestList.Add(r);
            }
            requests = requestList.ToArray();
        }
    }

}





/*
Mi resultado para el ejemplo:
Ganancia optima es $1328
Solicitudes asignadas a vehiculo 0:  1 3 6 10 13 16
Solicitudes asignadas a vehiculo 1:  2 8 12 17
*/



/*
Solicitud #1 asignada a vehiculo 0
  Minuto 0: parte desde km 10
  Minuto 5: llega a km 5
  Minuto 75: recoge pasajero de solicitud #1 en km 5
  Minuto 79: llega a km 9 y desmonta pasajero de la solicitud #1
  Tarifa de viaje: 50

Solicitud 2 asignada a vehiculo 1
  Minuto 0: parte desde km 10
  Minuto 20: llega a km 30
  Minuto 90: recoge pasajero de solicitud #2 en km 30
  Minuto 102: llega a km 18 y desmonta pasajero de la solicitud #2
  Tarifa de viaje: 92

Solicitud #3 asignada a vehiculo 0
  Minuto 79: parte desde km 9
  Minuto 85: llega a km 15
  Minuto 100: recoge pasajero de solicitud #3 en km 15
  Minuto 110: llega a km 25 y desmonta pasajero de la solicitud #3
  Tarifa de viaje: 80

Solicitud #6 asignada a vehiculo 0
  Minuto 110: parte desde km 25
  Minuto 128: llega a km 7
  Minuto 132: recoge pasajero de solicitud #6 en km 7
  Minuto 154: llega a km 29 y desmonta pasajero de la solicitud #6
  Tarifa de viaje: 152

Solicitud 8 asignada a vehiculo 1
  Minuto 102: parte desde km 18
  Minuto 136: llega a km 52
  Minuto 150: recoge pasajero de solicitud #8 en km 52
  Minuto 171: llega a km 31 y desmonta pasajero de la solicitud #8
  Tarifa de viaje: 146

Solicitud #10 asignada a vehiculo 0
  Minuto 154: parte desde km 29
  Minuto 158: llega a km 25
  Minuto 177: recoge pasajero de solicitud #10 en km 25
  Minuto 192: llega a km 10 y desmonta pasajero de la solicitud #10
  Tarifa de viaje: 110

Solicitud 12 asignada a vehiculo 1
  Minuto 171: parte desde km 31
  Minuto 190: llega a km 50
  Minuto 200: recoge pasajero de solicitud #12 en km 50
  Minuto 230: llega a km 20 y desmonta pasajero de la solicitud #12
  Tarifa de viaje: 200

Solicitud #13 asignada a vehiculo 0
  Minuto 192: parte desde km 10
  Minuto 207: llega a km 25
  Minuto 209: recoge pasajero de solicitud #13 en km 25
  Minuto 224: llega a km 10 y desmonta pasajero de la solicitud #13
  Tarifa de viaje: 110

Solicitud #16 asignada a vehiculo 0
  Minuto 224: parte desde km 10
  Minuto 256: llega a km 42
  Minuto 260: recoge pasajero de solicitud #16 en km 42
  Minuto 289: llega a km 13 y desmonta pasajero de la solicitud #16
  Tarifa de viaje: 194

Solicitud 17 asignada a vehiculo 1
  Minuto 230: parte desde km 20
  Minuto 252: llega a km 42
  Minuto 260: recoge pasajero de solicitud #17 en km 42
  Minuto 289: llega a km 13 y desmonta pasajero de la solicitud #17
  Tarifa de viaje: 194

*/
