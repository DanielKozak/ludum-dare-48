using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public static TutorialController Instance;
    float tutorialDelay = 10f;

    public bool isActive = true;

    private void Awake() => Instance = this;
    public void KillTutorial()
    {
        for (var i = 0; i < ToastController.Instance.ToastHistory.Count; i++)
        {
            if (ToastController.Instance.ToastHistory[i] != null)
                ToastController.Instance.ToastHistory[i].Kill();
        }
        isActive = false;
    }

    public IEnumerator StartTutorialRoutine()
    {

        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("Hi there! Welcome to the Jungle! I'm a tutorial. If you dont want to see me - press a button on the left.", true, true);
        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("You can move around the map using the middle mouse button or with WASD, and zoom with mouse wheel", true, true);

        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("You can also build stuff by using the build menu at the bottom.", true, true);

        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("To see info about a building or a truck  - select it with Left Mouse Click. Upgrades are there too", true, true);

        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("Upgrades to various stats are expencive, but powerful. Use them Wisely", true, true);

        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("Trucks are used to haul your stuff around the base, but move only on concrete (old model, sadly)", true, true);

        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("To order a truck around - Right Mouse Click on a building to see options or on concrete to move there.", true, true);

        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("Truck commands can be queued using Left Shift", true, true);

        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("Place concrete roads to move around or as a foundation for your buildings", true, true);

        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("Use Banana Extractors to collect bananas. The jungle will run out eventually, making room fore MORE roads", true, true);


        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("Extractors need fuel (banana oil), which is made by Refineries from...right, Bananas", true, true);


        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("Drop probes from orbit to reveal banana concentrations on the map.", true, true);


        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("You can sell spare stuff to the container ship anchored at your base, or top up on fuel if you have to", true, true);

        yield return new WaitForSeconds(tutorialDelay);
        ToastController.Instance.Toast("There were supposed to be hoards of monkeys making your life miserable, but sadly they did not make it in 72 hours :(", true, true);
        if (!isActive) yield break;


        yield return new WaitForSeconds(tutorialDelay);
        if (!isActive) yield break;
        ToastController.Instance.Toast("Your ultimate goal is to collect THE GOLDEN BANANA found in a temple deep in the jungle.", true, true);

        yield return new WaitForSeconds(tutorialDelay);
        ToastController.Instance.Toast("And to have fun of course! Good Luck!", true, true);
    }
}
