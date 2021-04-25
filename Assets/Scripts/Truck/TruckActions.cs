public class TruckActions
{

    public class MoveTo : TruckAction
    {
        public MoveTo(Truck agent, (int, int) target)
        {
            type = 1;
            location = target;
        }
        public override void OnCompleted()
        {
            UnityEngine.Debug.Log("Driving complete");
        }
    }
    public class Load : TruckAction
    {
        public Load((int, int) location, int cargoType)
        {

        }
        public override void OnCompleted()
        {
            UnityEngine.Debug.Log("Order complete");
        }
    }
    public class Unload : TruckAction
    {
        public Unload((int, int) location, int cargoType)
        {

        }
        public override void OnCompleted()
        {
            UnityEngine.Debug.Log("Order complete");
        }
    }


}