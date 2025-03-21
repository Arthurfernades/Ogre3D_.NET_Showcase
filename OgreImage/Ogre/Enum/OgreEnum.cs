using System.ComponentModel;

namespace OgreImage
{
    public enum OgreConstant
    {
        [Description("Default")]
        MSN_DEFAULT,

        [Description("ShaderGeneratorDefaultScheme")]
        MSN_SHADERGEN,

        [Description("NormalMap")]
        SRS_NORMALMAP
    }

    public enum TextureUsage
    {
        TU_STATIC = HardwareBufferUsage.HBU_GPU_TO_CPU,
        
        TU_DYNAMIC = HardwareBufferUsage.HBU_CPU_ONLY,
        
        TU_WRITE_ONLY = HardwareBufferUsage.HBU_DETAIL_WRITE_ONLY,
        
        TU_STATIC_WRITE_ONLY = HardwareBufferUsage.HBU_GPU_ONLY,
        
        TU_DYNAMIC_WRITE_ONLY = HardwareBufferUsage.HBU_CPU_TO_GPU,
        
        TU_DYNAMIC_WRITE_ONLY_DISCARDABLE = HardwareBufferUsage.HBU_CPU_TO_GPU,
        
        TU_AUTOMIPMAP = 0x10,
        
        TU_RENDERTARGET = 0x20,
        
        TU_NOT_SAMPLED = 0x40,
        
        TU_UNORDERED_ACCESS = 0x80,
        
        TU_DEFAULT = TU_AUTOMIPMAP | HardwareBufferUsage.HBU_GPU_ONLY,
        
        TU_UAV_NOT_SRV = TU_UNORDERED_ACCESS | TU_NOT_SAMPLED,
        
        TU_NOT_SRV = TU_NOT_SAMPLED,
        
        TU_NOTSHADERRESOURCE = TU_NOT_SAMPLED,
        
        TU_UAV = TU_UNORDERED_ACCESS
    }

    public enum HardwareBufferUsage : int
    {
        
        HBU_GPU_TO_CPU = 1,
        
        HBU_CPU_ONLY = 2,
        
        HBU_DETAIL_WRITE_ONLY = 4,
        
        HBU_GPU_ONLY = HBU_GPU_TO_CPU | HBU_DETAIL_WRITE_ONLY,

        HBU_CPU_TO_GPU = HBU_CPU_ONLY | HBU_DETAIL_WRITE_ONLY,
    };
}