using UnityEngine;
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
        }
        public override void Init()
        {
        }
    }
    public class Load : TruckAction
    {
        Truck currentAgent;
        Building currentDepot;
        int cargo;
        public Load(Truck agent, Building depot, (int, int) target, int cargoType)
        {

            location = target;
            currentAgent = agent;
            currentDepot = depot;
            cargo = cargoType;

        }
        public override void Init()
        {
            int aval = 0;
            switch (cargo)
            {
                case -1:
                    ((Ship)currentDepot).BuyOil(currentAgent.MaxLoad);
                    currentAgent.SetCargoType(1);

                    timer = currentAgent.MaxLoad / currentAgent.LoadingSpeed;
                    amount = currentAgent.MaxLoad;
                    return;
                case 0:
                    aval = ((BananaExtractor)currentDepot).BananaCount;
                    break;
                case 1:
                    aval = ((Refinery)currentDepot).currentProduct;
                    break;
                case 99:
                    ToastController.Instance.Toast("WIN");

                    return;
            }
            int space = currentAgent.MaxLoad - currentAgent.CurrentLoad;
            int req;
            if (aval >= space) req = currentAgent.MaxLoad;
            else req = aval;
            currentAgent.SetCargoType(cargo);

            timer = req / currentAgent.LoadingSpeed;
            amount = req;
        }
        public override void OnCompleted()
        {

            switch (cargo)
            {
                case 0:
                    ((BananaExtractor)currentDepot).SetBananaCount(-amount);
                    currentAgent.CurrentLoad += amount;
                    break;
                case 1:
                    ((Refinery)currentDepot).SetProduct(-amount);
                    currentAgent.CurrentLoad += amount;

                    break;
            }

            UnityEngine.Debug.Log("Order Load complete");
        }

    }
    public class Unload : TruckAction
    {
        Truck currentAgent;
        Building currentDepot;
        int cargo;
        public Unload(Truck agent, Building depot, (int, int) target, int cargoType)
        {

            location = target;
            currentAgent = agent;
            currentDepot = depot;
            cargo = cargoType;


        }
        public override void OnCompleted()
        {
            if (currentDepot.isRefinery)
            {
                ((Refinery)currentDepot).SetBananaCount(+amount);
            }
            if (currentDepot.isExtractor)
            {
                ((BananaExtractor)currentDepot).SetFuel(+amount);
            }
            if (currentDepot.isShip)
            {
                ((Ship)currentDepot).Sell(amount, cargo);
            }
        }
        public override void Init()
        {

            amount = currentAgent.CurrentLoad;

            timer = amount / currentAgent.LoadingSpeed;

            currentAgent.SetCargoType(-1);
        }
    }
    public class Win : TruckAction
    {
        public Win(Truck agent, (int, int) target)
        {
            location = target;
            timer = 0f;
        }
        public override void OnCompleted()
        {
            //TODO WIN

        }
        public override void Init()
        {
            UnityEngine.Debug.Log("Order complete");
        }
    }
}