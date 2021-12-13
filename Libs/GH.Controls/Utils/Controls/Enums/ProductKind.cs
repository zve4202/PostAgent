using System;

namespace GH.Controls.Utils.Controls
{
    [Flags]
    public enum ProductKind : long
    {
        Default = 0,
        DXperienceWin = 1,
        XtraReports = 16, // 0x0000000000000010
        DemoWin = 8192, // 0x0000000000002000
        XPO = 32768, // 0x0000000000008000
        DXperienceASP = 33554432, // 0x0000000002000000
        XAF = 268435456, // 0x0000000010000000
        DXperienceWPF = 274877906944, // 0x0000004000000000
        DXperienceSliverlight = 549755813888, // 0x0000008000000000
        LightSwitchReports = 35184372088832, // 0x0000200000000000
        Dashboard = 140737488355328, // 0x0000800000000000
        CodedUIWin = 281474976710656, // 0x0001000000000000
        Snap = 562949953421312, // 0x0002000000000000
        Docs = 36028797018963968, // 0x0080000000000000
        XtraReportsWpf = 144115188075855872, // 0x0200000000000000
        XtraReportsSL = 288230376151711744, // 0x0400000000000000
        XtraReportsWeb = 576460752303423488, // 0x0800000000000000
        XtraReportsWin = 1152921504606846976, // 0x1000000000000000
        FreeOffer = 4611686018427387904, // 0x4000000000000000
        DXperiencePro = Snap | XtraReports | DXperienceWin, // 0x0002000000000011
        DXperienceEnt = DXperiencePro | Docs | DXperienceSliverlight | DXperienceWPF | DXperienceASP | XPO, // 0x008200C002008011
        DXperienceUni = DXperienceEnt | Dashboard | XAF, // 0x008280C012008011
    }

}
