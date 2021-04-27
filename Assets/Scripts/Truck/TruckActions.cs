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
            int aval = 999;
            UnityEngine.Debug.Log($" AAAAAAAAAAAAAAAAA amount  {aval}");

            switch (cargo)
            {
                case -1:
                    Debug.LogError("truck load again broke");
                    break;
                case 0:
                    AudioManager.PlaySoundLocal(currentAgent.audioSource, "load_bananas");

                    if (currentDepot.isRefinery)
                        aval = ((Refinery)currentDepot).BananaCount;
                    else if (currentDepot.isExtractor)
                        aval = ((BananaExtractor)currentDepot).BananaCount;
                    break;
                case 1:
                    AudioManager.PlaySoundLocal(currentAgent.audioSource, "load_fuel");

                    if (currentDepot.isRefinery)
                        aval = ((Refinery)currentDepot).currentProduct;
                    else if (currentDepot.isExtractor)
                        aval = ((BananaExtractor)currentDepot).remainingFuel;
                    else if (currentDepot.isShip)
                        ((Ship)currentDepot).BuyOil(currentAgent.MaxLoad);
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
            // UnityEngine.Debug.Log($" cargo  {cargo}, agent cargo = {currentAgent.CargoType}");

            timer = req / currentAgent.LoadingSpeed;
            amount = req;
            // UnityEngine.Debug.Log($" AAAAAAAAAAAAAAAAA amount  {amount}, agent cargo = {currentAgent.CargoType}");

        }
        public override void OnCompleted()
        {

            switch (cargo)
            {
                case 0:
                    if (currentDepot.isRefinery)
                        ((Refinery)currentDepot).SetBananaCount(-amount);
                    else if (currentDepot.isExtractor)
                        ((BananaExtractor)currentDepot).SetBananaCount(-amount);

                    currentAgent.CurrentLoad += amount;
                    break;
                case 1:
                    if (currentDepot.isRefinery)
                        ((Refinery)currentDepot).SetProduct(-amount);
                    else if (currentDepot.isExtractor)
                        ((BananaExtractor)currentDepot).SetFuel(-amount);

                    currentAgent.CurrentLoad += amount;

                    break;
            }
            UnityEngine.Debug.Log($"agent load {currentAgent.CurrentLoad}");

            UnityEngine.Debug.Log("Order Load complete");
            currentDepot.ShowInfo();
            currentAgent.ShowInfo();


        }

    }
    public class Unload : TruckAction
    {
        Truck currentAgent;
        Building currentDepot;
        int cargo;
        public Unload(Truck agent, Building depot, (int, int) target, int newCargoType)
        {

            location = target;
            currentAgent = agent;
            currentDepot = depot;


        }
        public override void OnCompleted()
        {
            switch (currentAgent.CargoType)
            {
                case 0:

                    if (currentDepot.isRefinery)
                        ((Refinery)currentDepot).SetBananaCount(+amount);
                    else if (currentDepot.isExtractor)
                        ((BananaExtractor)currentDepot).SetBananaCount(+amount);
                    else if (currentDepot.isShip)
                        ((Ship)currentDepot).Sell(currentAgent.CurrentLoad, 0);
                    break;
                case 1:

                    if (currentDepot.isRefinery)
                        ((Refinery)currentDepot).SetProduct(+amount);
                    else if (currentDepot.isExtractor)
                        ((BananaExtractor)currentDepot).SetFuel(+amount);
                    else if (currentDepot.isShip)
                        ((Ship)currentDepot).Sell(currentAgent.CurrentLoad, 1);
                    break;
            }

            currentAgent.SetCargoType(-1);
            currentAgent.CurrentLoad -= amount;
            currentAgent.ShowInfo();

            currentDepot.ShowInfo();
        }
        public override void Init()
        {

            amount = currentAgent.CurrentLoad;
            if (currentAgent.CargoType == 1) AudioManager.PlaySoundLocal(currentAgent.audioSource, "load_fuel");

            timer = amount / currentAgent.LoadingSpeed;

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