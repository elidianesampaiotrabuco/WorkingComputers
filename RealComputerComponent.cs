using Rewired;
using System.Linq;
using UnityEngine;
using ZenFulcrum.EmbeddedBrowser;
using System.Collections;
using MTM101BaldAPI.Reflection;
using UnityEngine.UI;
using MTM101BaldAPI.AssetTools;

namespace WorkingComputers
{
    //this is all atrocious

    public class RealComputerComponent : MonoBehaviour, IClickable<int>
    {
        ComputerAudioManager audioManager = new ComputerAudioManager();
        Image reticle;
        ComputerUser user = ComputerUser.None;
        GameObject browserDisplay;
        SoundObject audNo = Resources.FindObjectsOfTypeAll<SoundObject>().ToList().First(x => x.name == "ErrorMaybe");
        SoundObject audUse = Resources.FindObjectsOfTypeAll<SoundObject>().ToList().First(x => x.name == "SwingDoorLock");
        int userPlayer;
        Transform cameraTarget;
        TimeScaleModifier npcStopper = new TimeScaleModifier(0f, 0f, 1f);
        Browser browser;
        bool checkForEgg;

        public void ClickableSighted(int player)
        {
        }

        public void ClickableUnsighted(int player)
        {
        }

        public bool ClickableHidden()
        {
            return false;
        }

        public bool ClickableRequiresNormalHeight()
        {
            return true;
        }
        public void Clicked(int player)
        {
            if (user != ComputerUser.None)
            {
                CoreGameManager.Instance.audMan.PlaySingle(this.audNo);
                return;
            }
            browserDisplay.GetComponent<PointerUIMesh>().enableMouseInput = true;
            CoreGameManager.Instance.audMan.PlaySingle(this.audUse);
            userPlayer = player;
            user = ComputerUser.Player;
            reticle = (Image)CoreGameManager.Instance.GetHud(userPlayer).ReflectionGetVariable("reticle");
            reticle.gameObject.SetActive(false);
            browserDisplay.GetComponent<PointerUIMesh>().viewCamera = CoreGameManager.Instance.GetCamera(userPlayer).camCom;
            browserDisplay.GetComponent<MeshCollider>().enabled = true;
            browser.UpdateCursor();
            CoreGameManager.Instance.GetPlayer(userPlayer).plm.Entity.SetInteractionState(false);
            CoreGameManager.Instance.GetPlayer(userPlayer).plm.Entity.SetFrozen(true);
            CoreGameManager.Instance.GetCamera(userPlayer).SetControllable(false);
            CoreGameManager.Instance.GetCamera(userPlayer).UpdateTargets(this.cameraTarget, 20);
            CursorManager.Instance.UnlockCursor();
            CoreGameManager.Instance.disablePause = true;
            StartCoroutine(ComputerLoop(CoreGameManager.Instance.GetPlayer(userPlayer)));
            browserDisplay.GetComponent<PointerUIMesh>().viewCamera = CoreGameManager.Instance.GetCamera(0).camCom;
            //freeze time
            if (WorkingComputersPlugin.plugin.timeFreeze.Value) CoreGameManager.Instance.GetPlayer(userPlayer).ec.AddTimeScale(npcStopper);
        }

        public void ExitComputerPlayer()
        {
            user = ComputerUser.None;
            reticle.gameObject.SetActive(true);
            browserDisplay.GetComponent<PointerUIMesh>().viewCamera = null;
            browserDisplay.GetComponent<MeshCollider>().enabled = false;
            browser.UpdateCursor(); 
            CoreGameManager.Instance.GetCamera(userPlayer).UpdateTargets(null, 20);
            CoreGameManager.Instance.GetPlayer(userPlayer).plm.Entity.SetInteractionState(true);
            CoreGameManager.Instance.GetPlayer(userPlayer).plm.Entity.SetFrozen(false);
            CoreGameManager.Instance.GetCamera(userPlayer).SetControllable(true);
            //unfreeze time
            if (WorkingComputersPlugin.plugin.timeFreeze.Value) CoreGameManager.Instance.GetPlayer(userPlayer).ec.RemoveTimeScale(npcStopper);
            CursorManager.Instance.LockCursor();
            CoreGameManager.Instance.disablePause = false;
            userPlayer = -1;
        }

        IEnumerator ComputerLoop(PlayerManager player)
        {
            while (user == ComputerUser.Player)
            {
                //leave
                if (Input.GetKeyDown(WorkingComputersPlugin.plugin.leaveKey.Value))
                {
                    ExitComputerPlayer();
                }
                //reset
                if (Input.GetKeyDown(WorkingComputersPlugin.plugin.resetKey.Value))
                {
                    browserDisplay.GetComponent<Browser>().Url = WorkingComputersPlugin.plugin.homePage.Value;
                }
                //previous
                if (Input.GetKeyDown(WorkingComputersPlugin.plugin.previousKey.Value))
                {
                    browserDisplay.GetComponent<Browser>().GoBack();
                }

                //check for easter egg
                if (WorkingComputersPlugin.plugin.easterEggs.Value)
                {
                    if (browser.IsLoadingRaw) checkForEgg = false;  
                    if (browser.IsLoaded && !checkForEgg)
                    {
                        SiteEasterEggManager.Instance.TryForEasterEgg(browser.Url, player);
                        checkForEgg = true;
                    }
                }
                yield return null;
            }
        }

        void Start()
        {
            //i guess we're doing this manually every time
            browserDisplay = GameObject.CreatePrimitive(PrimitiveType.Quad);
            browserDisplay.transform.parent = transform;
            browserDisplay.transform.localScale = new Vector3(2.45f, 1.675f, 1f);
            browserDisplay.transform.localPosition = new Vector3(-1.505f, 1.94f, 0f);
            Quaternion rotation = Quaternion.identity;
            rotation.eulerAngles = new Vector3(0f, 90f, 0f);
            browserDisplay.transform.localRotation = rotation;

            //browser component
            browser = browserDisplay.AddComponent<Browser>();
            browser.Url = WorkingComputersPlugin.plugin.homePage.Value;
            browser.Resize(1280, 840);
            browser.baseColor = Color.white;
            browserDisplay.GetComponent<MeshRenderer>().material = gameObject.GetComponent<MeshRenderer>().material;
            browserDisplay.GetComponent<MeshRenderer>().material.SetTexture("_LightGuide", AssetLoader.TextureFromMod(WorkingComputersPlugin.plugin, "ScreenLightmap.png"));
            browserDisplay.name = "BrowserDisplay";
            browserDisplay.GetComponent<MeshCollider>().enabled = false;

            //camera transform
            GameObject ctTemp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ctTemp.name = "CameraTransform";
            ctTemp.transform.parent = transform;
            ctTemp.transform.localPosition = new Vector3(-3.9f, 1.76f, 0f);
            ctTemp.transform.localRotation = rotation;
            cameraTarget = ctTemp.transform;
        }
    }
}
