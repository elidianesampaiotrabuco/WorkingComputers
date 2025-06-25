using Rewired;
using System.Linq;
using UnityEngine;
using ZenFulcrum.EmbeddedBrowser;
using System.Collections;
using MTM101BaldAPI.Reflection;
using UnityEngine.UI;

namespace WorkingComputers
{
    //this is all atrocious

    public class RealComputerComponent : MonoBehaviour, IClickable<int>
    {
        Image reticle;
        ComputerUser user = ComputerUser.None;
        GameObject browserDisplay;
        SoundObject audNo = Resources.FindObjectsOfTypeAll<SoundObject>().ToList().First(x => x.name == "ErrorMaybe");
        SoundObject audUse = Resources.FindObjectsOfTypeAll<SoundObject>().ToList().First(x => x.name == "SwingDoorLock");
        int userPlayer;
        Transform cameraTarget;
        TimeScaleModifier npcStopper = new TimeScaleModifier(0f,0f,1f);

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
            browserDisplay.GetComponent<MeshCollider>().enabled = true;
            CoreGameManager.Instance.GetPlayer(userPlayer).plm.Entity.SetInteractionState(false);
            CoreGameManager.Instance.GetPlayer(userPlayer).plm.Entity.SetFrozen(true);
            CoreGameManager.Instance.GetCamera(userPlayer).SetControllable(false);
            CoreGameManager.Instance.GetCamera(userPlayer).UpdateTargets(this.cameraTarget, 20);
            CursorManager.Instance.UnlockCursor();
            CoreGameManager.Instance.disablePause = true;
            StartCoroutine(CameraFocus(CoreGameManager.Instance.GetPlayer(userPlayer)));
            
            //freeze time
            if (WorkingComputersPlugin.plugin.timeFreeze.Value) CoreGameManager.Instance.GetPlayer(userPlayer).ec.AddTimeScale(npcStopper);
        }

        public void ExitComputerPlayer()
        {
            user = ComputerUser.None;
            reticle.gameObject.SetActive(true);
            browserDisplay.GetComponent<MeshCollider>().enabled = false;
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

        IEnumerator CameraFocus(PlayerManager player)
        {
            while (user == ComputerUser.Player)
            {
                if (InputManager.Instance.GetDigitalInput("Pause", true) && user == ComputerUser.Player)
                {
                    ExitComputerPlayer();
                }
                //reset
                if (Input.GetKeyDown(KeyCode.Home) && browserDisplay.GetComponent<Browser>().Url != "https://google.com")
                {
                    browserDisplay.GetComponent<Browser>().Url = "https://google.com";
                }
                //previous
                if (Input.GetKeyDown(KeyCode.Insert))
                {
                    browserDisplay.GetComponent<Browser>().GoBack();
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
            Browser browserComp = browserDisplay.AddComponent<Browser>();
            browserComp.Url = "https://google.com";
            browserComp.Resize(1280, 820);
            browserComp.baseColor = Color.white;
            browserDisplay.GetComponent<MeshRenderer>().material = gameObject.GetComponent<MeshRenderer>().material;
            browserDisplay.GetComponent<MeshRenderer>().material.SetTexture("_LightGuide", Resources.FindObjectsOfTypeAll<Texture2D>().ToList().First(x => x.name == "WhiteTexture"));
            browserDisplay.name = "BrowserDisplay";
            browserDisplay.GetComponent<MeshCollider>().enabled = false;

            //camera transform
            GameObject ctTemp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ctTemp.name = "CameraTransform";
            ctTemp.transform.parent = transform;
            ctTemp.transform.localPosition = new Vector3(-3.9f, 1.76f, 0f);
            ctTemp.transform.localRotation = rotation;
            cameraTarget = ctTemp.transform;

            browserDisplay.GetComponent<PointerUIMesh>().viewCamera = CoreGameManager.Instance.GetCamera(0).camCom;
        }
    }
}
