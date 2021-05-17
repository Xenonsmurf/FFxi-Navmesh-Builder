// ***********************************************************************
// Assembly         : FFXI NAVMESH BUILDER
// Author           : Xenonsmurf
// Created          : 04-29-2021
//
// Last Modified By : Xenonsmurf
// Last Modified On : 04-26-2021
// ***********************************************************************
// <copyright file="ResourceType.cs" company="Xenonsmurf">
//     Copyright © Xenonsmurf 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Ffxi_Navmesh_Builder.Common.dat.Types
{
    /// <summary>
    /// Enum ResourceType
    /// </summary>
    public enum ResourceType : byte
    {
        /// <summary>
        /// The terminate
        /// </summary>
        Terminate = 0x00,
        /// <summary>
        /// The RMP
        /// </summary>
        Rmp = 0x01,
        /// <summary>
        /// The RMW
        /// </summary>
        Rmw = 0x02,
        /// <summary>
        /// The directory
        /// </summary>
        Directory = 0x03,
        /// <summary>
        /// The bin
        /// </summary>
        Bin = 0x04,
        /// <summary>
        /// The generator
        /// </summary>
        Generator = 0x05,
        /// <summary>
        /// The camera
        /// </summary>
        Camera = 0x06,
        /// <summary>
        /// The scheduler
        /// </summary>
        Scheduler = 0x07,
        /// <summary>
        /// The MTX
        /// </summary>
        Mtx = 0x08,
        /// <summary>
        /// The tim
        /// </summary>
        Tim = 0x09,
        /// <summary>
        /// The texinfo
        /// </summary>
        Texinfo = 0x0A,
        /// <summary>
        /// The vum
        /// </summary>
        Vum = 0x0B,
        /// <summary>
        /// The om1
        /// </summary>
        Om1 = 0x0C,
        /// <summary>
        /// The file information
        /// </summary>
        FileInfo = 0x0D,
        /// <summary>
        /// The anm
        /// </summary>
        Anm = 0x0E,
        /// <summary>
        /// The RSD
        /// </summary>
        Rsd = 0x0F,
        /// <summary>
        /// The unknown
        /// </summary>
        Unknown = 0x10,
        /// <summary>
        /// The osm
        /// </summary>
        Osm = 0x11,
        /// <summary>
        /// The SKD
        /// </summary>
        Skd = 0x12,
        /// <summary>
        /// The MTD
        /// </summary>
        Mtd = 0x13,
        /// <summary>
        /// The MLD
        /// </summary>
        Mld = 0x14,
        /// <summary>
        /// The MLT
        /// </summary>
        Mlt = 0x15,
        /// <summary>
        /// The MWS
        /// </summary>
        Mws = 0x16,
        /// <summary>
        /// The mod
        /// </summary>
        Mod = 0x17,
        /// <summary>
        /// The tim2
        /// </summary>
        Tim2 = 0x18,
        /// <summary>
        /// The keyframe
        /// </summary>
        Keyframe = 0x19,
        /// <summary>
        /// The BMP
        /// </summary>
        Bmp = 0x1A,
        /// <summary>
        /// The BMP2
        /// </summary>
        Bmp2 = 0x1B,
        /// <summary>
        /// The MZB
        /// </summary>
        Mzb = 0x1C,
        /// <summary>
        /// The MMD
        /// </summary>
        Mmd = 0x1D,
        /// <summary>
        /// The mep
        /// </summary>
        Mep = 0x1E,
        /// <summary>
        /// The d3 m
        /// </summary>
        D3M = 0x1F,
        /// <summary>
        /// The d3 s
        /// </summary>
        D3S = 0x20,
        /// <summary>
        /// The d3 a
        /// </summary>
        D3A = 0x21,
        /// <summary>
        /// The dist prog
        /// </summary>
        DistProg = 0x22,
        /// <summary>
        /// The vu line prog
        /// </summary>
        VuLineProg = 0x23,
        /// <summary>
        /// The ring prog
        /// </summary>
        RingProg = 0x24,
        /// <summary>
        /// The d3 b
        /// </summary>
        D3B = 0x25,
        /// <summary>
        /// The asn
        /// </summary>
        Asn = 0x26,
        /// <summary>
        /// The mot
        /// </summary>
        Mot = 0x27,
        /// <summary>
        /// The SKL
        /// </summary>
        Skl = 0x28,
        /// <summary>
        /// The SK2
        /// </summary>
        Sk2 = 0x29,
        /// <summary>
        /// The os2
        /// </summary>
        Os2 = 0x2A,
        /// <summary>
        /// The mo2
        /// </summary>
        Mo2 = 0x2B,
        /// <summary>
        /// The PSW
        /// </summary>
        Psw = 0x2C,
        /// <summary>
        /// The WSD
        /// </summary>
        Wsd = 0x2D,
        /// <summary>
        /// The MMB
        /// </summary>
        Mmb = 0x2E,
        /// <summary>
        /// The weather
        /// </summary>
        Weather = 0x2F,
        /// <summary>
        /// The meb
        /// </summary>
        Meb = 0x30,
        /// <summary>
        /// The MSB
        /// </summary>
        Msb = 0x31,
        /// <summary>
        /// The med
        /// </summary>
        Med = 0x32,
        /// <summary>
        /// The MSH
        /// </summary>
        Msh = 0x33,
        /// <summary>
        /// The ysh
        /// </summary>
        Ysh = 0x34,
        /// <summary>
        /// The MBP
        /// </summary>
        Mbp = 0x35,
        /// <summary>
        /// The rid
        /// </summary>
        Rid = 0x36,
        /// <summary>
        /// The wb
        /// </summary>
        Wb = 0x37,
        /// <summary>
        /// The BGM
        /// </summary>
        Bgm = 0x38,
        /// <summary>
        /// The LFD
        /// </summary>
        Lfd = 0x39,
        /// <summary>
        /// The lfe
        /// </summary>
        Lfe = 0x3A,
        /// <summary>
        /// The esh
        /// </summary>
        Esh = 0x3B,
        /// <summary>
        /// The SCH
        /// </summary>
        Sch = 0x3C,
        /// <summary>
        /// The sep
        /// </summary>
        Sep = 0x3D,
        /// <summary>
        /// The VTX
        /// </summary>
        Vtx = 0x3E,
        /// <summary>
        /// The lwo
        /// </summary>
        Lwo = 0x3F,
        /// <summary>
        /// The rme
        /// </summary>
        Rme = 0x40,
        /// <summary>
        /// The elt
        /// </summary>
        Elt = 0x41,
        /// <summary>
        /// The rab
        /// </summary>
        Rab = 0x42,
        /// <summary>
        /// The MTT
        /// </summary>
        Mtt = 0x43,
        /// <summary>
        /// The MTB
        /// </summary>
        Mtb = 0x44,
        /// <summary>
        /// The cib
        /// </summary>
        Cib = 0x45,
        /// <summary>
        /// The TLT
        /// </summary>
        Tlt = 0x46,
        /// <summary>
        /// The point light prog
        /// </summary>
        PointLightProg = 0x47,
        /// <summary>
        /// The MGD
        /// </summary>
        Mgd = 0x48,
        /// <summary>
        /// The MGB
        /// </summary>
        Mgb = 0x49,
        /// <summary>
        /// The SPH
        /// </summary>
        Sph = 0x4A,
        /// <summary>
        /// The BMD
        /// </summary>
        Bmd = 0x4B,
        /// <summary>
        /// The qif
        /// </summary>
        Qif = 0x4C,
        /// <summary>
        /// The QDT
        /// </summary>
        Qdt = 0x4D,
        /// <summary>
        /// The mif
        /// </summary>
        Mif = 0x4E,
        /// <summary>
        /// The MDT
        /// </summary>
        Mdt = 0x4F,
        /// <summary>
        /// The sif
        /// </summary>
        Sif = 0x50,
        /// <summary>
        /// The SDT
        /// </summary>
        Sdt = 0x51,
        /// <summary>
        /// The acd
        /// </summary>
        Acd = 0x52,
        /// <summary>
        /// The acb
        /// </summary>
        Acb = 0x53,
        /// <summary>
        /// The afb
        /// </summary>
        Afb = 0x54,
        /// <summary>
        /// The aft
        /// </summary>
        Aft = 0x55,
        /// <summary>
        /// The WWD
        /// </summary>
        Wwd = 0x56,
        /// <summary>
        /// The null prog
        /// </summary>
        NullProg = 0x57,
        /// <summary>
        /// The SPW
        /// </summary>
        Spw = 0x58,
        /// <summary>
        /// The fud
        /// </summary>
        Fud = 0x59,
        /// <summary>
        /// The disgregater prog
        /// </summary>
        DisgregaterProg = 0x5A,
        /// <summary>
        /// The SMT
        /// </summary>
        Smt = 0x5B,
        /// <summary>
        /// The dam value prog
        /// </summary>
        DamValueProg = 0x5C,
        /// <summary>
        /// The bp
        /// </summary>
        Bp = 0x5D,
    }
}