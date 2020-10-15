using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;
using System.IO;

public class CameraContrllor : MonoBehaviour {

    public  Camera      mainCamera;
    public  GameObject  wholeCamera;
    public  GameObject  firstCamera;
    public  GameObject  secondCamera;
    public  GameObject  parentCamera;
    public  GameObject  renderCamera;
    public  GameObject  moveBase;
    public  GameObject  fPS_UI;
    public  Slider      slider_dOF;
    public  Slider      slider_fOV;
    public  Slider      slider_fL;
    public  Slider      slider_H;
    public  Slider      slider_L;
    public  Slider      slider_MB;
    public  int         xMax;
    public  int         xMin;
    public  int         yMax;
    public  int         yMin;
    public  float       speed;
    public  bool        wolkAllowed;

    public  Shader      outShader;
    public  Texture     inputTex;
    //public Texture      tempTex;


    private PostProcessingBehaviour     cameraPostProcessing;
    private DepthOfFieldModel.Settings  dOF_Model;
    private MotionBlurModel.Settings    mB_Model;
    private Vector3                     oriRotation;
    private Vector3                     oriMouseRotation;
    private bool                        isRotate                    =false;
    private bool                        isSwitch                    =true;
    // private int                         numOfPhoto                  =0;
    private float                         trueLight;


    private void Awake()                
    {
       cameraPostProcessing= mainCamera.GetComponent<PostProcessingBehaviour>();

        dOF_Model = new DepthOfFieldModel.Settings();
        mB_Model = new MotionBlurModel.Settings();


        dOF_Model = cameraPostProcessing.profile.depthOfField.settings;
        mB_Model = cameraPostProcessing.profile.motionBlur.settings;
        oriRotation = wholeCamera.transform.localEulerAngles;
        oriMouseRotation = Input.mousePosition;

        ChangeDOFValue();
        ChangeFLValue();
        ChangeFOVValue();
        //ChangeH();
        ChangeLValue();
        ChangeMBValue();
        ChangeTrueLightValue();

    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift))        //Shift开关判定
        {
            oriRotation = wholeCamera.transform.localEulerAngles;
            isRotate = !isRotate;  //开关
            oriMouseRotation = Input.mousePosition;
        }
        /*if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRotate = false;
            oriRotation = wholeCamera.transform.localEulerAngles;
        }*/

        Debug.Log((mainCamera.transform.localEulerAngles.x * Mathf.Sign(Input.mousePosition.y - oriMouseRotation.y)));
        


    }




    private void FixedUpdate()
    {
        if (isRotate)
            MoveCamera();

        if(wolkAllowed)
        Walk();
    }



    public void SwitchCamera()
    {
        if (isSwitch)
        {
            secondCamera.SetActive(true);
            firstCamera.SetActive(false);
            fPS_UI.SetActive(true);
            //mainCamera.gameObject.SetActive(false);
            isSwitch = !isSwitch;
        }
        else
        {
            fPS_UI.SetActive(false);
            //mainCamera.gameObject.SetActive(true);
            firstCamera.SetActive(true);
            secondCamera.SetActive(false);
            isSwitch = !isSwitch;
        }
    }

    public void MoveCamera()
    {
       // if(Angle_Judge())
        //mainCamera.transform.localEulerAngles = new Vector3 (oriRotation.x -(Input.mousePosition.y-oriMouseRotation.y), oriRotation.y + (Input.mousePosition.x - oriMouseRotation.x), 0);

        wholeCamera.transform.localEulerAngles = new Vector3(oriRotation.x - speed * (Input.mousePosition.y - oriMouseRotation.y), oriRotation.y + speed*(Input.mousePosition.x - oriMouseRotation.x),0);
    }

    public void Walk()
    {
        if (Input.GetKey(KeyCode.W))
        {
            // Vector3 dir = wholeCamera.transform.TransformDirection(wholeCamera.transform.forward);
            moveBase.transform.Translate(wholeCamera.transform.forward * Time.deltaTime * -10f, Space.Self);
            //会跳，要优化一下
        }
        if (Input.GetKey(KeyCode.A))
        {
            // Vector3 dir = wholeCamera.transform.TransformDirection(wholeCamera.transform.forward);
            moveBase.transform.Translate(wholeCamera.transform.right * Time.deltaTime * 10f, Space.Self);
        }
        if (Input.GetKey(KeyCode.S))
        {
            // Vector3 dir = wholeCamera.transform.TransformDirection(wholeCamera.transform.forward);
            moveBase.transform.Translate(wholeCamera.transform.forward * Time.deltaTime * 10f, Space.Self);
        }
        if (Input.GetKey(KeyCode.D))
        {
            // Vector3 dir = wholeCamera.transform.TransformDirection(wholeCamera.transform.forward);
            moveBase.transform.Translate(wholeCamera.transform.right * Time.deltaTime * -10f, Space.Self);
        }
    }

    private bool Angle_Judge()  //角度判定，待优化
    {
        if(oriRotation.x - (Input.mousePosition.y - oriMouseRotation.y) > xMax && (Input.mousePosition.y - oriMouseRotation.y) < 0){
            return false;
        }else if(oriRotation.x - (Input.mousePosition.y - oriMouseRotation.y) < xMin && (Input.mousePosition.y - oriMouseRotation.y)>0){
            return false;
        }
        else if (oriRotation.y - (Input.mousePosition.x - oriMouseRotation.x) < yMin && (Input.mousePosition.x - oriMouseRotation.x) > 0){
            return false;
        }
        else if (oriRotation.y - (Input.mousePosition.x - oriMouseRotation.x) > yMax && (Input.mousePosition.x - oriMouseRotation.x) < 0){
            return false;
        }

        return true;

    }



    public void ChangeDOFValue()        //改变景深
    {
        dOF_Model.focusDistance = slider_dOF.value*-5f;
        cameraPostProcessing.profile.depthOfField.settings=dOF_Model;
        slider_dOF.transform.Find("Text (1)").GetComponent<Text>().text = ((1f-(Mathf.Abs(slider_dOF.value) / (slider_dOF.maxValue - slider_dOF.minValue))) * 1000f).ToString()+"mm";
    }

    public void ChangeFOVValue()        //改变视野
    {
        mainCamera.fieldOfView = slider_fOV.value;
        secondCamera.GetComponent<Camera>().fieldOfView = slider_fOV.value;
        slider_fOV.transform.Find("Text (1)").GetComponent<Text>().text = ((((Mathf.Abs((slider_fOV.value)- slider_fOV.minValue) / (slider_fOV.maxValue - slider_fOV.minValue))) * 153f)+3f).ToString() + "°";
    }


    public void ChangeFLValue()     //改变光圈
    {
        dOF_Model.focalLength = slider_fL.value * 50f;
        cameraPostProcessing.profile.depthOfField.settings = dOF_Model;
        ChangeTrueLightValue();
        string f="0";
        switch ((int)slider_fL.value)
        {
            case 12:
                f = "1";
                break;
            case 11:
                f = "1.4";
                break;
            case 10:
                f = "2";
                break;
            case 9:
                f = "2.8";
                break;
            case 8:
                f = "4";
                break;
            case 7:
                f = "5.6";
                break;
            case 6:
                f = "8";
                break;
            case 5:
                f = "11";
                break;
            case 4:
                f = "16";
                break;
            case 3:
                f = "22";
                break;
            case 2:
                f = "32";
                break;
            case 1:
                f = "45";
                break;
            case 0:
                f = "64";
                break;
        }
        slider_fL.transform.Find("Text (1)").GetComponent<Text>().text = f + "F";

    }
    public void ChangeH()     //改变高度
    {
        parentCamera.transform.position = new Vector3(parentCamera.transform.position.x,slider_H.value,parentCamera.transform.position.z);
    }

    public void Photo()
    {

        
        SaveRenderTextureToPNG(inputTex, outShader, Application.dataPath + "/temp", System.DateTime.Now.Day*1000000+ System.DateTime.Now.Hour*10000+ System.DateTime.Now.Minute*100+ System.DateTime.Now.Second + "Photo");
        

    }

    public bool SaveRenderTextureToPNG(Texture inputTex, Shader outputShader, string contents, string pngName)
    {

        

        RenderTexture temp = RenderTexture.GetTemporary(1360, 1080, 0, RenderTextureFormat.ARGB32);
        Material mat = new Material(outputShader);
        Graphics.Blit(inputTex, temp, mat);
        bool ret = SaveRenderTextureToPNG(temp, contents, pngName);
        RenderTexture.ReleaseTemporary(temp);
        
        return ret;
        

    }




    //将RenderTexture保存成一张png图片  
    public bool SaveRenderTextureToPNG(RenderTexture rt, string contents, string pngName)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        byte[] bytes = png.EncodeToPNG();
        if (!Directory.Exists(contents))
            Directory.CreateDirectory(contents);
        FileStream file = File.Open(contents + "/" + pngName + ".png", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();
        Texture2D.DestroyImmediate(png);
        png = null;
        RenderTexture.active = prev;
        
        return true;

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeLValue()        //改变曝光
    {
        /*mainCamera.gameObject.GetComponent<Tonemapping>().exposureAdjustment = slider_L.value;
        secondCamera.GetComponent<Tonemapping>().exposureAdjustment = slider_L.value;*/
        ChangeTrueLightValue();
        string f = "0";
        switch ((int)slider_L.value)
        {
            case -3:
                f = "12";
                break;
            case -2:
                f = "25";
                break;
            case -1:
                f = "50";
                break;
            case 0:
                f = "100";
                break;
            case 1:
                f = "200";
                break;
            case 2:
                f = "400";
                break;
            case 3:
                f = "800";
                break;
            case 4:
                f = "1600";
                break;
           
        }
        slider_L.transform.Find("Text (1)").GetComponent<Text>().text = "ISO"+f;
    }

    private void ChangeTrueLightValue()
    {
        trueLight = (float)(slider_L.value+4)*2f * ((float)(slider_fL.value+0.5f)/10f )* ((float)(slider_MB.value+0.5f)/10f);
        mainCamera.gameObject.GetComponent<Tonemapping>().exposureAdjustment = trueLight;
        secondCamera.GetComponent<Tonemapping>().exposureAdjustment = trueLight;
    }

    public void ChangeMBValue()        //改变曝光时间
    {
        mainCamera.gameObject.GetComponent<MotionBlur>().blurAmount = (slider_MB.value/12)*0.9f;
        secondCamera.GetComponent<MotionBlur>().blurAmount = (slider_MB.value / 12) * 0.9f;
        ChangeTrueLightValue();
        string f = "0";
        switch ((int)slider_MB.value)
        {
            case 12:
                f = "1";
                break;
            case 11:
                f = "1/2";
                break;
            case 10:
                f = "1/4";
                break;
            case 9:
                f = "1/8";
                break;
            case 8:
                f = "1/15";
                break;
            case 7:
                f = "1/30";
                break;
            case 6:
                f = "1/60";
                break;
            case 5:
                f = "1/125";
                break;
            case 4:
                f = "1/250";
                break;
            case 3:
                f = "1/500";
                break;
            case 2:
                f = "1/1000";
                break;
            case 1:
                f = "1/2000";
                break;
            case 0:
                f = "1/4000";
                break;
        }
        slider_MB.transform.Find("Text (1)").GetComponent<Text>().text = f + "s";

    }

    public void SwitchScene1()        //切换场景
    {
        SceneManager.LoadScene("main");

    }

    public void SwitchScene0()        //切换场景
    {
        SceneManager.LoadScene("main 1");

    }
}  

