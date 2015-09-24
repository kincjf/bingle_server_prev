using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bingle.Server.Data.APSConfig
{
    /// <summary>
    /// Stitching된 output의 품질을 설정함.
    /// </summary>
    public enum JPEGQuality
    {
        Low1 = 1,
        Low2,
        Low3,
        Low4,
        Medium1,
        Medium2,
        Medium3,
        High1,
        High2,
        Maximum1,
        Maximum2,
        Maximum3
    }

    public enum PNGQuality
    {
        Low1 = 1,
        Low2,
        Medium1,
        Medium2,
        High1,
        High2,
        Maximum
    }

    /// <summary>
    /// AutoPano Server(APS) 작동을 위한 XML 형식 지정
    /// @seealso AutopanoServer 4.0(pdf) - Output formats 참조
    /// </summary>
    [XmlRootAttribute("aps")]
    public class APSConfig
    {
        [XmlElement("activation")]
        public APSActivation activation;

        [XmlElement("group")]
        public APSGroup group;

        [XmlElement("application")]
        public APSapplication application;

        [XmlElement("pano")]
        public APSPano pano;

        public APSConfig() { }

        /// <summary>
        /// example(AutopanoServer, BingleServer/Debug 기준)
        /// - ../../AutopanoServer/AutopanoServer xml=./Temp/[folderPath]/[folderPath].xml
        /// - 기본 설정 : inputFolderPath와 rendered image의 이름이 같다.
        /// </summary>
        /// <param name="inputFolderPath">image folder path</param>
        /// <param name="outputFolderPath">rendering path </param>
        public APSConfig(string inputFolderPath, string outputFolderPath)
        {
            activation = new APSActivation();

            application = new APSapplication();
            application.inputFolder = inputFolderPath;

            group = new APSGroup();

            pano = new APSPano();
            pano.renderFolderTpl = outputFolderPath;

            Console.WriteLine("APSConfig - InputFolder : " + inputFolderPath
                + ", RenderFolder : " + outputFolderPath);      //for test
        }
    }

    //[XmlElement("activation")]
    public class APSActivation
    {
        public string code = "W54T-2DNI-YVGY-CB8E-HSZK-DJGZ-JSTA";
        public bool userInfo = true;
    }

    //[XmlElement("group")]
    public class APSGroup
    {
        [XmlElement("level_linkMode")]
        public ushort levelLinkMode = 0;

        [XmlElement("stack_linkMode")]
        public ushort stackLinkMode = 1;
    }

    //[XmlElement("application")]
    public class APSapplication
    {
        public string inputFolder;

        [XmlElement("detection_templateMode")]
        public ushort detectionTemplateMode = 0;

        /// <summary>
        /// false: multiple panoramas will be created if not all the images are linked together (default)
        /// true: Force every image to be in one panorama
        /// </summary>
        [XmlElement("isolate_allinone")]
        public bool isolateAllinone = true;

        /// <summary>
        /// not provided or 0: no log
        /// 1: for logging messages in the console
        /// 2: for logging messages in a file. The filename will have the same name as the rendered panorama added with .log as suffix
        /// 3: for logging messages in the console and in the file
        /// </summary>
        [XmlElement("Log")]
        public ushort log = 0;
    }

    /// <summary>
    /// "jpg", "png"
    /// default : jpg
    /// </summary>
    //[XmlElement("pano")]
    public class APSPano
    {
        /// <summary>
        /// rendering path
        /// </summary>
        [XmlElement("render_folderTpl")]
        public string renderFolderTpl = String.Empty;

        /// <summary>
        /// denotes the name of the generated image. Some special variables can be used: 
        /// %a : Name of the project
        /// </summary>
        [XmlElement("render_filenameTpl")]
        public string renderFilenameTpl = "%a";

        /// <summary>
        /// denotes the path where the generated .pano files will be saved
        /// %i : Use image folder as destination folder
        /// </summary>
        [XmlElement("project_folderTpl")]
        public string projectFolderTpl = "%i";

        /// <summary>
        /// denotes the name of generated .pano files.
        /// %R0 : Use image folder name
        /// </summary>
        [XmlElement("project_filenameTpl")]
        public string projectFilenameTpl = "%R0";

        [XmlElement("render_flletype")]
        public string renderFileType = "jpg";

        [XmlElement("auto_Save")]
        public bool autoSave = true;

        [XmlElement("auto_Render")]
        public bool autoRender = true;

        [XmlElement("render_percent")]
        public ushort renderPercent = 100;

        [XmlElement("render_fileCompression")]
        public ushort renderFileCompression = (ushort)JPEGQuality.Maximum1;

        [XmlElement("render_interpolation")]
        public ushort renderInterpolation = 2;

        [XmlElement("render_graphcut")]
        public ushort renderGraphcut = 1;

        [XmlElement("render_blendAlgo")]
        public ushort renderBlendAlgo = 2;

        [XmlElement("render_multiband_level")]
        public short renderBlend = -2;

        /// <summary>
        /// 0: no correction (default), 1: laguerre, 2: hdri
        /// </summary>
        [XmlElement("colorEqMode")]
        public ushort colorEqMode = 0;

        [XmlElement("auto_Color")]
        public bool autoColor = true;

        /// <summary>
        /// -1 : automatic
        /// 0: spherical
        /// </summary>
        [XmlElement("projection")]
        public ushort projection = 0;

        /// <summary>
        /// fit Image size as projection type
        /// equirectangular - w:h = 2:1 
        /// </summary>
        [XmlElement("geo_fitMode")]
        public ushort geoFitMode = 2;
    }
}
