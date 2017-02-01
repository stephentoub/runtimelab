// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// File implements Slicing-by-8 CRC Generation, as described in
// "Novel Table Lookup-Based Algorithms for High-Performance CRC Generation"
// IEEE TRANSACTIONS ON COMPUTERS, VOL. 57, NO. 11, NOVEMBER 2008

/*
 * Copyright (c) 2004-2006 Intel Corporation - All Rights Reserved
 *
 *
 * This software program is licensed subject to the BSD License,
 * available at http://www.opensource.org/licenses/bsd-license.html.
 */

using System.Diagnostics;

namespace System.IO.Compression
{
    /// <summary>
    /// This class contains a managed Crc32 function as well as an indirection to the Interop.Zlib.Crc32 call.
    /// Since Desktop compression uses this file alongside the Open ZipArchive, we cannot remove it
    /// without breaking the Desktop build.
    ///
    /// Note that in CoreFX the ZlibCrc32 function is always called.
    /// </summary>
    internal static class Crc32Helper
    {
        // Calculate CRC based on the old CRC and the new bytes
        // See RFC1952 for details.
        public static uint UpdateCrc32(uint crc32, byte[] buffer, int offset, int length)
        {
            Debug.Assert((buffer != null) && (offset >= 0) && (length >= 0)
                       && (offset <= buffer.Length - length), "check the caller");
#if FEATURE_ZLIB
            return Interop.zlib.crc32(crc32, buffer, offset, length);
#else
            return ManagedCrc32(crc32, buffer, offset, length);
#endif

        }

#if !FEATURE_ZLIB

        // Generated tables for managed crc calculation.
        // Each table n (starting at 0) contains remainders from the long division of
        // all possible byte values, shifted by an offset of (n * 4 bits).
        // The divisor used is the crc32 standard polynomial 0xEDB88320
        // Please see cited paper for more details.
        private static readonly uint[] s_crcTable_0 = new uint[256] {
            0x00000000u, 0x77073096u, 0xee0e612cu, 0x990951bau, 0x076dc419u,
            0x706af48fu, 0xe963a535u, 0x9e6495a3u, 0x0edb8832u, 0x79dcb8a4u,
            0xe0d5e91eu, 0x97d2d988u, 0x09b64c2bu, 0x7eb17cbdu, 0xe7b82d07u,
            0x90bf1d91u, 0x1db71064u, 0x6ab020f2u, 0xf3b97148u, 0x84be41deu,
            0x1adad47du, 0x6ddde4ebu, 0xf4d4b551u, 0x83d385c7u, 0x136c9856u,
            0x646ba8c0u, 0xfd62f97au, 0x8a65c9ecu, 0x14015c4fu, 0x63066cd9u,
            0xfa0f3d63u, 0x8d080df5u, 0x3b6e20c8u, 0x4c69105eu, 0xd56041e4u,
            0xa2677172u, 0x3c03e4d1u, 0x4b04d447u, 0xd20d85fdu, 0xa50ab56bu,
            0x35b5a8fau, 0x42b2986cu, 0xdbbbc9d6u, 0xacbcf940u, 0x32d86ce3u,
            0x45df5c75u, 0xdcd60dcfu, 0xabd13d59u, 0x26d930acu, 0x51de003au,
            0xc8d75180u, 0xbfd06116u, 0x21b4f4b5u, 0x56b3c423u, 0xcfba9599u,
            0xb8bda50fu, 0x2802b89eu, 0x5f058808u, 0xc60cd9b2u, 0xb10be924u,
            0x2f6f7c87u, 0x58684c11u, 0xc1611dabu, 0xb6662d3du, 0x76dc4190u,
            0x01db7106u, 0x98d220bcu, 0xefd5102au, 0x71b18589u, 0x06b6b51fu,
            0x9fbfe4a5u, 0xe8b8d433u, 0x7807c9a2u, 0x0f00f934u, 0x9609a88eu,
            0xe10e9818u, 0x7f6a0dbbu, 0x086d3d2du, 0x91646c97u, 0xe6635c01u,
            0x6b6b51f4u, 0x1c6c6162u, 0x856530d8u, 0xf262004eu, 0x6c0695edu,
            0x1b01a57bu, 0x8208f4c1u, 0xf50fc457u, 0x65b0d9c6u, 0x12b7e950u,
            0x8bbeb8eau, 0xfcb9887cu, 0x62dd1ddfu, 0x15da2d49u, 0x8cd37cf3u,
            0xfbd44c65u, 0x4db26158u, 0x3ab551ceu, 0xa3bc0074u, 0xd4bb30e2u,
            0x4adfa541u, 0x3dd895d7u, 0xa4d1c46du, 0xd3d6f4fbu, 0x4369e96au,
            0x346ed9fcu, 0xad678846u, 0xda60b8d0u, 0x44042d73u, 0x33031de5u,
            0xaa0a4c5fu, 0xdd0d7cc9u, 0x5005713cu, 0x270241aau, 0xbe0b1010u,
            0xc90c2086u, 0x5768b525u, 0x206f85b3u, 0xb966d409u, 0xce61e49fu,
            0x5edef90eu, 0x29d9c998u, 0xb0d09822u, 0xc7d7a8b4u, 0x59b33d17u,
            0x2eb40d81u, 0xb7bd5c3bu, 0xc0ba6cadu, 0xedb88320u, 0x9abfb3b6u,
            0x03b6e20cu, 0x74b1d29au, 0xead54739u, 0x9dd277afu, 0x04db2615u,
            0x73dc1683u, 0xe3630b12u, 0x94643b84u, 0x0d6d6a3eu, 0x7a6a5aa8u,
            0xe40ecf0bu, 0x9309ff9du, 0x0a00ae27u, 0x7d079eb1u, 0xf00f9344u,
            0x8708a3d2u, 0x1e01f268u, 0x6906c2feu, 0xf762575du, 0x806567cbu,
            0x196c3671u, 0x6e6b06e7u, 0xfed41b76u, 0x89d32be0u, 0x10da7a5au,
            0x67dd4accu, 0xf9b9df6fu, 0x8ebeeff9u, 0x17b7be43u, 0x60b08ed5u,
            0xd6d6a3e8u, 0xa1d1937eu, 0x38d8c2c4u, 0x4fdff252u, 0xd1bb67f1u,
            0xa6bc5767u, 0x3fb506ddu, 0x48b2364bu, 0xd80d2bdau, 0xaf0a1b4cu,
            0x36034af6u, 0x41047a60u, 0xdf60efc3u, 0xa867df55u, 0x316e8eefu,
            0x4669be79u, 0xcb61b38cu, 0xbc66831au, 0x256fd2a0u, 0x5268e236u,
            0xcc0c7795u, 0xbb0b4703u, 0x220216b9u, 0x5505262fu, 0xc5ba3bbeu,
            0xb2bd0b28u, 0x2bb45a92u, 0x5cb36a04u, 0xc2d7ffa7u, 0xb5d0cf31u,
            0x2cd99e8bu, 0x5bdeae1du, 0x9b64c2b0u, 0xec63f226u, 0x756aa39cu,
            0x026d930au, 0x9c0906a9u, 0xeb0e363fu, 0x72076785u, 0x05005713u,
            0x95bf4a82u, 0xe2b87a14u, 0x7bb12baeu, 0x0cb61b38u, 0x92d28e9bu,
            0xe5d5be0du, 0x7cdcefb7u, 0x0bdbdf21u, 0x86d3d2d4u, 0xf1d4e242u,
            0x68ddb3f8u, 0x1fda836eu, 0x81be16cdu, 0xf6b9265bu, 0x6fb077e1u,
            0x18b74777u, 0x88085ae6u, 0xff0f6a70u, 0x66063bcau, 0x11010b5cu,
            0x8f659effu, 0xf862ae69u, 0x616bffd3u, 0x166ccf45u, 0xa00ae278u,
            0xd70dd2eeu, 0x4e048354u, 0x3903b3c2u, 0xa7672661u, 0xd06016f7u,
            0x4969474du, 0x3e6e77dbu, 0xaed16a4au, 0xd9d65adcu, 0x40df0b66u,
            0x37d83bf0u, 0xa9bcae53u, 0xdebb9ec5u, 0x47b2cf7fu, 0x30b5ffe9u,
            0xbdbdf21cu, 0xcabac28au, 0x53b39330u, 0x24b4a3a6u, 0xbad03605u,
            0xcdd70693u, 0x54de5729u, 0x23d967bfu, 0xb3667a2eu, 0xc4614ab8u,
            0x5d681b02u, 0x2a6f2b94u, 0xb40bbe37u, 0xc30c8ea1u, 0x5a05df1bu,
            0x2d02ef8du
        };
        private static readonly uint[] s_crcTable_1 = new uint[256] {
            0x00000000u, 0x191B3141u, 0x32366282u, 0x2B2D53C3u, 0x646CC504u,
            0x7D77F445u, 0x565AA786u, 0x4F4196C7u, 0xC8D98A08u, 0xD1C2BB49u,
            0xFAEFE88Au, 0xE3F4D9CBu, 0xACB54F0Cu, 0xB5AE7E4Du, 0x9E832D8Eu,
            0x87981CCFu, 0x4AC21251u, 0x53D92310u, 0x78F470D3u, 0x61EF4192u,
            0x2EAED755u, 0x37B5E614u, 0x1C98B5D7u, 0x05838496u, 0x821B9859u,
            0x9B00A918u, 0xB02DFADBu, 0xA936CB9Au, 0xE6775D5Du, 0xFF6C6C1Cu,
            0xD4413FDFu, 0xCD5A0E9Eu, 0x958424A2u, 0x8C9F15E3u, 0xA7B24620u,
            0xBEA97761u, 0xF1E8E1A6u, 0xE8F3D0E7u, 0xC3DE8324u, 0xDAC5B265u,
            0x5D5DAEAAu, 0x44469FEBu, 0x6F6BCC28u, 0x7670FD69u, 0x39316BAEu,
            0x202A5AEFu, 0x0B07092Cu, 0x121C386Du, 0xDF4636F3u, 0xC65D07B2u,
            0xED705471u, 0xF46B6530u, 0xBB2AF3F7u, 0xA231C2B6u, 0x891C9175u,
            0x9007A034u, 0x179FBCFBu, 0x0E848DBAu, 0x25A9DE79u, 0x3CB2EF38u,
            0x73F379FFu, 0x6AE848BEu, 0x41C51B7Du, 0x58DE2A3Cu, 0xF0794F05u,
            0xE9627E44u, 0xC24F2D87u, 0xDB541CC6u, 0x94158A01u, 0x8D0EBB40u,
            0xA623E883u, 0xBF38D9C2u, 0x38A0C50Du, 0x21BBF44Cu, 0x0A96A78Fu,
            0x138D96CEu, 0x5CCC0009u, 0x45D73148u, 0x6EFA628Bu, 0x77E153CAu,
            0xBABB5D54u, 0xA3A06C15u, 0x888D3FD6u, 0x91960E97u, 0xDED79850u,
            0xC7CCA911u, 0xECE1FAD2u, 0xF5FACB93u, 0x7262D75Cu, 0x6B79E61Du,
            0x4054B5DEu, 0x594F849Fu, 0x160E1258u, 0x0F152319u, 0x243870DAu,
            0x3D23419Bu, 0x65FD6BA7u, 0x7CE65AE6u, 0x57CB0925u, 0x4ED03864u,
            0x0191AEA3u, 0x188A9FE2u, 0x33A7CC21u, 0x2ABCFD60u, 0xAD24E1AFu,
            0xB43FD0EEu, 0x9F12832Du, 0x8609B26Cu, 0xC94824ABu, 0xD05315EAu,
            0xFB7E4629u, 0xE2657768u, 0x2F3F79F6u, 0x362448B7u, 0x1D091B74u,
            0x04122A35u, 0x4B53BCF2u, 0x52488DB3u, 0x7965DE70u, 0x607EEF31u,
            0xE7E6F3FEu, 0xFEFDC2BFu, 0xD5D0917Cu, 0xCCCBA03Du, 0x838A36FAu,
            0x9A9107BBu, 0xB1BC5478u, 0xA8A76539u, 0x3B83984Bu, 0x2298A90Au,
            0x09B5FAC9u, 0x10AECB88u, 0x5FEF5D4Fu, 0x46F46C0Eu, 0x6DD93FCDu,
            0x74C20E8Cu, 0xF35A1243u, 0xEA412302u, 0xC16C70C1u, 0xD8774180u,
            0x9736D747u, 0x8E2DE606u, 0xA500B5C5u, 0xBC1B8484u, 0x71418A1Au,
            0x685ABB5Bu, 0x4377E898u, 0x5A6CD9D9u, 0x152D4F1Eu, 0x0C367E5Fu,
            0x271B2D9Cu, 0x3E001CDDu, 0xB9980012u, 0xA0833153u, 0x8BAE6290u,
            0x92B553D1u, 0xDDF4C516u, 0xC4EFF457u, 0xEFC2A794u, 0xF6D996D5u,
            0xAE07BCE9u, 0xB71C8DA8u, 0x9C31DE6Bu, 0x852AEF2Au, 0xCA6B79EDu,
            0xD37048ACu, 0xF85D1B6Fu, 0xE1462A2Eu, 0x66DE36E1u, 0x7FC507A0u,
            0x54E85463u, 0x4DF36522u, 0x02B2F3E5u, 0x1BA9C2A4u, 0x30849167u,
            0x299FA026u, 0xE4C5AEB8u, 0xFDDE9FF9u, 0xD6F3CC3Au, 0xCFE8FD7Bu,
            0x80A96BBCu, 0x99B25AFDu, 0xB29F093Eu, 0xAB84387Fu, 0x2C1C24B0u,
            0x350715F1u, 0x1E2A4632u, 0x07317773u, 0x4870E1B4u, 0x516BD0F5u,
            0x7A468336u, 0x635DB277u, 0xCBFAD74Eu, 0xD2E1E60Fu, 0xF9CCB5CCu,
            0xE0D7848Du, 0xAF96124Au, 0xB68D230Bu, 0x9DA070C8u, 0x84BB4189u,
            0x03235D46u, 0x1A386C07u, 0x31153FC4u, 0x280E0E85u, 0x674F9842u,
            0x7E54A903u, 0x5579FAC0u, 0x4C62CB81u, 0x8138C51Fu, 0x9823F45Eu,
            0xB30EA79Du, 0xAA1596DCu, 0xE554001Bu, 0xFC4F315Au, 0xD7626299u,
            0xCE7953D8u, 0x49E14F17u, 0x50FA7E56u, 0x7BD72D95u, 0x62CC1CD4u,
            0x2D8D8A13u, 0x3496BB52u, 0x1FBBE891u, 0x06A0D9D0u, 0x5E7EF3ECu,
            0x4765C2ADu, 0x6C48916Eu, 0x7553A02Fu, 0x3A1236E8u, 0x230907A9u,
            0x0824546Au, 0x113F652Bu, 0x96A779E4u, 0x8FBC48A5u, 0xA4911B66u,
            0xBD8A2A27u, 0xF2CBBCE0u, 0xEBD08DA1u, 0xC0FDDE62u, 0xD9E6EF23u,
            0x14BCE1BDu, 0x0DA7D0FCu, 0x268A833Fu, 0x3F91B27Eu, 0x70D024B9u,
            0x69CB15F8u, 0x42E6463Bu, 0x5BFD777Au, 0xDC656BB5u, 0xC57E5AF4u,
            0xEE530937u, 0xF7483876u, 0xB809AEB1u, 0xA1129FF0u, 0x8A3FCC33u,
            0x9324FD72u
        };
        private static readonly uint[] s_crcTable_2 = new uint[256] {
            0x00000000u, 0x01C26A37u, 0x0384D46Eu, 0x0246BE59u, 0x0709A8DCu,
            0x06CBC2EBu, 0x048D7CB2u, 0x054F1685u, 0x0E1351B8u, 0x0FD13B8Fu,
            0x0D9785D6u, 0x0C55EFE1u, 0x091AF964u, 0x08D89353u, 0x0A9E2D0Au,
            0x0B5C473Du, 0x1C26A370u, 0x1DE4C947u, 0x1FA2771Eu, 0x1E601D29u,
            0x1B2F0BACu, 0x1AED619Bu, 0x18ABDFC2u, 0x1969B5F5u, 0x1235F2C8u,
            0x13F798FFu, 0x11B126A6u, 0x10734C91u, 0x153C5A14u, 0x14FE3023u,
            0x16B88E7Au, 0x177AE44Du, 0x384D46E0u, 0x398F2CD7u, 0x3BC9928Eu,
            0x3A0BF8B9u, 0x3F44EE3Cu, 0x3E86840Bu, 0x3CC03A52u, 0x3D025065u,
            0x365E1758u, 0x379C7D6Fu, 0x35DAC336u, 0x3418A901u, 0x3157BF84u,
            0x3095D5B3u, 0x32D36BEAu, 0x331101DDu, 0x246BE590u, 0x25A98FA7u,
            0x27EF31FEu, 0x262D5BC9u, 0x23624D4Cu, 0x22A0277Bu, 0x20E69922u,
            0x2124F315u, 0x2A78B428u, 0x2BBADE1Fu, 0x29FC6046u, 0x283E0A71u,
            0x2D711CF4u, 0x2CB376C3u, 0x2EF5C89Au, 0x2F37A2ADu, 0x709A8DC0u,
            0x7158E7F7u, 0x731E59AEu, 0x72DC3399u, 0x7793251Cu, 0x76514F2Bu,
            0x7417F172u, 0x75D59B45u, 0x7E89DC78u, 0x7F4BB64Fu, 0x7D0D0816u,
            0x7CCF6221u, 0x798074A4u, 0x78421E93u, 0x7A04A0CAu, 0x7BC6CAFDu,
            0x6CBC2EB0u, 0x6D7E4487u, 0x6F38FADEu, 0x6EFA90E9u, 0x6BB5866Cu,
            0x6A77EC5Bu, 0x68315202u, 0x69F33835u, 0x62AF7F08u, 0x636D153Fu,
            0x612BAB66u, 0x60E9C151u, 0x65A6D7D4u, 0x6464BDE3u, 0x662203BAu,
            0x67E0698Du, 0x48D7CB20u, 0x4915A117u, 0x4B531F4Eu, 0x4A917579u,
            0x4FDE63FCu, 0x4E1C09CBu, 0x4C5AB792u, 0x4D98DDA5u, 0x46C49A98u,
            0x4706F0AFu, 0x45404EF6u, 0x448224C1u, 0x41CD3244u, 0x400F5873u,
            0x4249E62Au, 0x438B8C1Du, 0x54F16850u, 0x55330267u, 0x5775BC3Eu,
            0x56B7D609u, 0x53F8C08Cu, 0x523AAABBu, 0x507C14E2u, 0x51BE7ED5u,
            0x5AE239E8u, 0x5B2053DFu, 0x5966ED86u, 0x58A487B1u, 0x5DEB9134u,
            0x5C29FB03u, 0x5E6F455Au, 0x5FAD2F6Du, 0xE1351B80u, 0xE0F771B7u,
            0xE2B1CFEEu, 0xE373A5D9u, 0xE63CB35Cu, 0xE7FED96Bu, 0xE5B86732u,
            0xE47A0D05u, 0xEF264A38u, 0xEEE4200Fu, 0xECA29E56u, 0xED60F461u,
            0xE82FE2E4u, 0xE9ED88D3u, 0xEBAB368Au, 0xEA695CBDu, 0xFD13B8F0u,
            0xFCD1D2C7u, 0xFE976C9Eu, 0xFF5506A9u, 0xFA1A102Cu, 0xFBD87A1Bu,
            0xF99EC442u, 0xF85CAE75u, 0xF300E948u, 0xF2C2837Fu, 0xF0843D26u,
            0xF1465711u, 0xF4094194u, 0xF5CB2BA3u, 0xF78D95FAu, 0xF64FFFCDu,
            0xD9785D60u, 0xD8BA3757u, 0xDAFC890Eu, 0xDB3EE339u, 0xDE71F5BCu,
            0xDFB39F8Bu, 0xDDF521D2u, 0xDC374BE5u, 0xD76B0CD8u, 0xD6A966EFu,
            0xD4EFD8B6u, 0xD52DB281u, 0xD062A404u, 0xD1A0CE33u, 0xD3E6706Au,
            0xD2241A5Du, 0xC55EFE10u, 0xC49C9427u, 0xC6DA2A7Eu, 0xC7184049u,
            0xC25756CCu, 0xC3953CFBu, 0xC1D382A2u, 0xC011E895u, 0xCB4DAFA8u,
            0xCA8FC59Fu, 0xC8C97BC6u, 0xC90B11F1u, 0xCC440774u, 0xCD866D43u,
            0xCFC0D31Au, 0xCE02B92Du, 0x91AF9640u, 0x906DFC77u, 0x922B422Eu,
            0x93E92819u, 0x96A63E9Cu, 0x976454ABu, 0x9522EAF2u, 0x94E080C5u,
            0x9FBCC7F8u, 0x9E7EADCFu, 0x9C381396u, 0x9DFA79A1u, 0x98B56F24u,
            0x99770513u, 0x9B31BB4Au, 0x9AF3D17Du, 0x8D893530u, 0x8C4B5F07u,
            0x8E0DE15Eu, 0x8FCF8B69u, 0x8A809DECu, 0x8B42F7DBu, 0x89044982u,
            0x88C623B5u, 0x839A6488u, 0x82580EBFu, 0x801EB0E6u, 0x81DCDAD1u,
            0x8493CC54u, 0x8551A663u, 0x8717183Au, 0x86D5720Du, 0xA9E2D0A0u,
            0xA820BA97u, 0xAA6604CEu, 0xABA46EF9u, 0xAEEB787Cu, 0xAF29124Bu,
            0xAD6FAC12u, 0xACADC625u, 0xA7F18118u, 0xA633EB2Fu, 0xA4755576u,
            0xA5B73F41u, 0xA0F829C4u, 0xA13A43F3u, 0xA37CFDAAu, 0xA2BE979Du,
            0xB5C473D0u, 0xB40619E7u, 0xB640A7BEu, 0xB782CD89u, 0xB2CDDB0Cu,
            0xB30FB13Bu, 0xB1490F62u, 0xB08B6555u, 0xBBD72268u, 0xBA15485Fu,
            0xB853F606u, 0xB9919C31u, 0xBCDE8AB4u, 0xBD1CE083u, 0xBF5A5EDAu,
            0xBE9834EDu
        };
        private static readonly uint[] s_crcTable_3 = new uint[256] {
            0x00000000u, 0xB8BC6765u, 0xAA09C88Bu, 0x12B5AFEEu, 0x8F629757u,
            0x37DEF032u, 0x256B5FDCu, 0x9DD738B9u, 0xC5B428EFu, 0x7D084F8Au,
            0x6FBDE064u, 0xD7018701u, 0x4AD6BFB8u, 0xF26AD8DDu, 0xE0DF7733u,
            0x58631056u, 0x5019579Fu, 0xE8A530FAu, 0xFA109F14u, 0x42ACF871u,
            0xDF7BC0C8u, 0x67C7A7ADu, 0x75720843u, 0xCDCE6F26u, 0x95AD7F70u,
            0x2D111815u, 0x3FA4B7FBu, 0x8718D09Eu, 0x1ACFE827u, 0xA2738F42u,
            0xB0C620ACu, 0x087A47C9u, 0xA032AF3Eu, 0x188EC85Bu, 0x0A3B67B5u,
            0xB28700D0u, 0x2F503869u, 0x97EC5F0Cu, 0x8559F0E2u, 0x3DE59787u,
            0x658687D1u, 0xDD3AE0B4u, 0xCF8F4F5Au, 0x7733283Fu, 0xEAE41086u,
            0x525877E3u, 0x40EDD80Du, 0xF851BF68u, 0xF02BF8A1u, 0x48979FC4u,
            0x5A22302Au, 0xE29E574Fu, 0x7F496FF6u, 0xC7F50893u, 0xD540A77Du,
            0x6DFCC018u, 0x359FD04Eu, 0x8D23B72Bu, 0x9F9618C5u, 0x272A7FA0u,
            0xBAFD4719u, 0x0241207Cu, 0x10F48F92u, 0xA848E8F7u, 0x9B14583Du,
            0x23A83F58u, 0x311D90B6u, 0x89A1F7D3u, 0x1476CF6Au, 0xACCAA80Fu,
            0xBE7F07E1u, 0x06C36084u, 0x5EA070D2u, 0xE61C17B7u, 0xF4A9B859u,
            0x4C15DF3Cu, 0xD1C2E785u, 0x697E80E0u, 0x7BCB2F0Eu, 0xC377486Bu,
            0xCB0D0FA2u, 0x73B168C7u, 0x6104C729u, 0xD9B8A04Cu, 0x446F98F5u,
            0xFCD3FF90u, 0xEE66507Eu, 0x56DA371Bu, 0x0EB9274Du, 0xB6054028u,
            0xA4B0EFC6u, 0x1C0C88A3u, 0x81DBB01Au, 0x3967D77Fu, 0x2BD27891u,
            0x936E1FF4u, 0x3B26F703u, 0x839A9066u, 0x912F3F88u, 0x299358EDu,
            0xB4446054u, 0x0CF80731u, 0x1E4DA8DFu, 0xA6F1CFBAu, 0xFE92DFECu,
            0x462EB889u, 0x549B1767u, 0xEC277002u, 0x71F048BBu, 0xC94C2FDEu,
            0xDBF98030u, 0x6345E755u, 0x6B3FA09Cu, 0xD383C7F9u, 0xC1366817u,
            0x798A0F72u, 0xE45D37CBu, 0x5CE150AEu, 0x4E54FF40u, 0xF6E89825u,
            0xAE8B8873u, 0x1637EF16u, 0x048240F8u, 0xBC3E279Du, 0x21E91F24u,
            0x99557841u, 0x8BE0D7AFu, 0x335CB0CAu, 0xED59B63Bu, 0x55E5D15Eu,
            0x47507EB0u, 0xFFEC19D5u, 0x623B216Cu, 0xDA874609u, 0xC832E9E7u,
            0x708E8E82u, 0x28ED9ED4u, 0x9051F9B1u, 0x82E4565Fu, 0x3A58313Au,
            0xA78F0983u, 0x1F336EE6u, 0x0D86C108u, 0xB53AA66Du, 0xBD40E1A4u,
            0x05FC86C1u, 0x1749292Fu, 0xAFF54E4Au, 0x322276F3u, 0x8A9E1196u,
            0x982BBE78u, 0x2097D91Du, 0x78F4C94Bu, 0xC048AE2Eu, 0xD2FD01C0u,
            0x6A4166A5u, 0xF7965E1Cu, 0x4F2A3979u, 0x5D9F9697u, 0xE523F1F2u,
            0x4D6B1905u, 0xF5D77E60u, 0xE762D18Eu, 0x5FDEB6EBu, 0xC2098E52u,
            0x7AB5E937u, 0x680046D9u, 0xD0BC21BCu, 0x88DF31EAu, 0x3063568Fu,
            0x22D6F961u, 0x9A6A9E04u, 0x07BDA6BDu, 0xBF01C1D8u, 0xADB46E36u,
            0x15080953u, 0x1D724E9Au, 0xA5CE29FFu, 0xB77B8611u, 0x0FC7E174u,
            0x9210D9CDu, 0x2AACBEA8u, 0x38191146u, 0x80A57623u, 0xD8C66675u,
            0x607A0110u, 0x72CFAEFEu, 0xCA73C99Bu, 0x57A4F122u, 0xEF189647u,
            0xFDAD39A9u, 0x45115ECCu, 0x764DEE06u, 0xCEF18963u, 0xDC44268Du,
            0x64F841E8u, 0xF92F7951u, 0x41931E34u, 0x5326B1DAu, 0xEB9AD6BFu,
            0xB3F9C6E9u, 0x0B45A18Cu, 0x19F00E62u, 0xA14C6907u, 0x3C9B51BEu,
            0x842736DBu, 0x96929935u, 0x2E2EFE50u, 0x2654B999u, 0x9EE8DEFCu,
            0x8C5D7112u, 0x34E11677u, 0xA9362ECEu, 0x118A49ABu, 0x033FE645u,
            0xBB838120u, 0xE3E09176u, 0x5B5CF613u, 0x49E959FDu, 0xF1553E98u,
            0x6C820621u, 0xD43E6144u, 0xC68BCEAAu, 0x7E37A9CFu, 0xD67F4138u,
            0x6EC3265Du, 0x7C7689B3u, 0xC4CAEED6u, 0x591DD66Fu, 0xE1A1B10Au,
            0xF3141EE4u, 0x4BA87981u, 0x13CB69D7u, 0xAB770EB2u, 0xB9C2A15Cu,
            0x017EC639u, 0x9CA9FE80u, 0x241599E5u, 0x36A0360Bu, 0x8E1C516Eu,
            0x866616A7u, 0x3EDA71C2u, 0x2C6FDE2Cu, 0x94D3B949u, 0x090481F0u,
            0xB1B8E695u, 0xA30D497Bu, 0x1BB12E1Eu, 0x43D23E48u, 0xFB6E592Du,
            0xE9DBF6C3u, 0x516791A6u, 0xCCB0A91Fu, 0x740CCE7Au, 0x66B96194u,
            0xDE0506F1u
        };
        private static readonly uint[] s_crcTable_4 = new uint[256] {
            0x00000000u, 0x3D6029B0u, 0x7AC05360u, 0x47A07AD0u, 0xF580A6C0u,
            0xC8E08F70u, 0x8F40F5A0u, 0xB220DC10u, 0x30704BC1u, 0x0D106271u,
            0x4AB018A1u, 0x77D03111u, 0xC5F0ED01u, 0xF890C4B1u, 0xBF30BE61u,
            0x825097D1u, 0x60E09782u, 0x5D80BE32u, 0x1A20C4E2u, 0x2740ED52u,
            0x95603142u, 0xA80018F2u, 0xEFA06222u, 0xD2C04B92u, 0x5090DC43u,
            0x6DF0F5F3u, 0x2A508F23u, 0x1730A693u, 0xA5107A83u, 0x98705333u,
            0xDFD029E3u, 0xE2B00053u, 0xC1C12F04u, 0xFCA106B4u, 0xBB017C64u,
            0x866155D4u, 0x344189C4u, 0x0921A074u, 0x4E81DAA4u, 0x73E1F314u,
            0xF1B164C5u, 0xCCD14D75u, 0x8B7137A5u, 0xB6111E15u, 0x0431C205u,
            0x3951EBB5u, 0x7EF19165u, 0x4391B8D5u, 0xA121B886u, 0x9C419136u,
            0xDBE1EBE6u, 0xE681C256u, 0x54A11E46u, 0x69C137F6u, 0x2E614D26u,
            0x13016496u, 0x9151F347u, 0xAC31DAF7u, 0xEB91A027u, 0xD6F18997u,
            0x64D15587u, 0x59B17C37u, 0x1E1106E7u, 0x23712F57u, 0x58F35849u,
            0x659371F9u, 0x22330B29u, 0x1F532299u, 0xAD73FE89u, 0x9013D739u,
            0xD7B3ADE9u, 0xEAD38459u, 0x68831388u, 0x55E33A38u, 0x124340E8u,
            0x2F236958u, 0x9D03B548u, 0xA0639CF8u, 0xE7C3E628u, 0xDAA3CF98u,
            0x3813CFCBu, 0x0573E67Bu, 0x42D39CABu, 0x7FB3B51Bu, 0xCD93690Bu,
            0xF0F340BBu, 0xB7533A6Bu, 0x8A3313DBu, 0x0863840Au, 0x3503ADBAu,
            0x72A3D76Au, 0x4FC3FEDAu, 0xFDE322CAu, 0xC0830B7Au, 0x872371AAu,
            0xBA43581Au, 0x9932774Du, 0xA4525EFDu, 0xE3F2242Du, 0xDE920D9Du,
            0x6CB2D18Du, 0x51D2F83Du, 0x167282EDu, 0x2B12AB5Du, 0xA9423C8Cu,
            0x9422153Cu, 0xD3826FECu, 0xEEE2465Cu, 0x5CC29A4Cu, 0x61A2B3FCu,
            0x2602C92Cu, 0x1B62E09Cu, 0xF9D2E0CFu, 0xC4B2C97Fu, 0x8312B3AFu,
            0xBE729A1Fu, 0x0C52460Fu, 0x31326FBFu, 0x7692156Fu, 0x4BF23CDFu,
            0xC9A2AB0Eu, 0xF4C282BEu, 0xB362F86Eu, 0x8E02D1DEu, 0x3C220DCEu,
            0x0142247Eu, 0x46E25EAEu, 0x7B82771Eu, 0xB1E6B092u, 0x8C869922u,
            0xCB26E3F2u, 0xF646CA42u, 0x44661652u, 0x79063FE2u, 0x3EA64532u,
            0x03C66C82u, 0x8196FB53u, 0xBCF6D2E3u, 0xFB56A833u, 0xC6368183u,
            0x74165D93u, 0x49767423u, 0x0ED60EF3u, 0x33B62743u, 0xD1062710u,
            0xEC660EA0u, 0xABC67470u, 0x96A65DC0u, 0x248681D0u, 0x19E6A860u,
            0x5E46D2B0u, 0x6326FB00u, 0xE1766CD1u, 0xDC164561u, 0x9BB63FB1u,
            0xA6D61601u, 0x14F6CA11u, 0x2996E3A1u, 0x6E369971u, 0x5356B0C1u,
            0x70279F96u, 0x4D47B626u, 0x0AE7CCF6u, 0x3787E546u, 0x85A73956u,
            0xB8C710E6u, 0xFF676A36u, 0xC2074386u, 0x4057D457u, 0x7D37FDE7u,
            0x3A978737u, 0x07F7AE87u, 0xB5D77297u, 0x88B75B27u, 0xCF1721F7u,
            0xF2770847u, 0x10C70814u, 0x2DA721A4u, 0x6A075B74u, 0x576772C4u,
            0xE547AED4u, 0xD8278764u, 0x9F87FDB4u, 0xA2E7D404u, 0x20B743D5u,
            0x1DD76A65u, 0x5A7710B5u, 0x67173905u, 0xD537E515u, 0xE857CCA5u,
            0xAFF7B675u, 0x92979FC5u, 0xE915E8DBu, 0xD475C16Bu, 0x93D5BBBBu,
            0xAEB5920Bu, 0x1C954E1Bu, 0x21F567ABu, 0x66551D7Bu, 0x5B3534CBu,
            0xD965A31Au, 0xE4058AAAu, 0xA3A5F07Au, 0x9EC5D9CAu, 0x2CE505DAu,
            0x11852C6Au, 0x562556BAu, 0x6B457F0Au, 0x89F57F59u, 0xB49556E9u,
            0xF3352C39u, 0xCE550589u, 0x7C75D999u, 0x4115F029u, 0x06B58AF9u,
            0x3BD5A349u, 0xB9853498u, 0x84E51D28u, 0xC34567F8u, 0xFE254E48u,
            0x4C059258u, 0x7165BBE8u, 0x36C5C138u, 0x0BA5E888u, 0x28D4C7DFu,
            0x15B4EE6Fu, 0x521494BFu, 0x6F74BD0Fu, 0xDD54611Fu, 0xE03448AFu,
            0xA794327Fu, 0x9AF41BCFu, 0x18A48C1Eu, 0x25C4A5AEu, 0x6264DF7Eu,
            0x5F04F6CEu, 0xED242ADEu, 0xD044036Eu, 0x97E479BEu, 0xAA84500Eu,
            0x4834505Du, 0x755479EDu, 0x32F4033Du, 0x0F942A8Du, 0xBDB4F69Du,
            0x80D4DF2Du, 0xC774A5FDu, 0xFA148C4Du, 0x78441B9Cu, 0x4524322Cu,
            0x028448FCu, 0x3FE4614Cu, 0x8DC4BD5Cu, 0xB0A494ECu, 0xF704EE3Cu,
            0xCA64C78Cu
        };
        private static readonly uint[] s_crcTable_5 = new uint[256] {
            0x00000000u, 0xCB5CD3A5u, 0x4DC8A10Bu, 0x869472AEu, 0x9B914216u,
            0x50CD91B3u, 0xD659E31Du, 0x1D0530B8u, 0xEC53826Du, 0x270F51C8u,
            0xA19B2366u, 0x6AC7F0C3u, 0x77C2C07Bu, 0xBC9E13DEu, 0x3A0A6170u,
            0xF156B2D5u, 0x03D6029Bu, 0xC88AD13Eu, 0x4E1EA390u, 0x85427035u,
            0x9847408Du, 0x531B9328u, 0xD58FE186u, 0x1ED33223u, 0xEF8580F6u,
            0x24D95353u, 0xA24D21FDu, 0x6911F258u, 0x7414C2E0u, 0xBF481145u,
            0x39DC63EBu, 0xF280B04Eu, 0x07AC0536u, 0xCCF0D693u, 0x4A64A43Du,
            0x81387798u, 0x9C3D4720u, 0x57619485u, 0xD1F5E62Bu, 0x1AA9358Eu,
            0xEBFF875Bu, 0x20A354FEu, 0xA6372650u, 0x6D6BF5F5u, 0x706EC54Du,
            0xBB3216E8u, 0x3DA66446u, 0xF6FAB7E3u, 0x047A07ADu, 0xCF26D408u,
            0x49B2A6A6u, 0x82EE7503u, 0x9FEB45BBu, 0x54B7961Eu, 0xD223E4B0u,
            0x197F3715u, 0xE82985C0u, 0x23755665u, 0xA5E124CBu, 0x6EBDF76Eu,
            0x73B8C7D6u, 0xB8E41473u, 0x3E7066DDu, 0xF52CB578u, 0x0F580A6Cu,
            0xC404D9C9u, 0x4290AB67u, 0x89CC78C2u, 0x94C9487Au, 0x5F959BDFu,
            0xD901E971u, 0x125D3AD4u, 0xE30B8801u, 0x28575BA4u, 0xAEC3290Au,
            0x659FFAAFu, 0x789ACA17u, 0xB3C619B2u, 0x35526B1Cu, 0xFE0EB8B9u,
            0x0C8E08F7u, 0xC7D2DB52u, 0x4146A9FCu, 0x8A1A7A59u, 0x971F4AE1u,
            0x5C439944u, 0xDAD7EBEAu, 0x118B384Fu, 0xE0DD8A9Au, 0x2B81593Fu,
            0xAD152B91u, 0x6649F834u, 0x7B4CC88Cu, 0xB0101B29u, 0x36846987u,
            0xFDD8BA22u, 0x08F40F5Au, 0xC3A8DCFFu, 0x453CAE51u, 0x8E607DF4u,
            0x93654D4Cu, 0x58399EE9u, 0xDEADEC47u, 0x15F13FE2u, 0xE4A78D37u,
            0x2FFB5E92u, 0xA96F2C3Cu, 0x6233FF99u, 0x7F36CF21u, 0xB46A1C84u,
            0x32FE6E2Au, 0xF9A2BD8Fu, 0x0B220DC1u, 0xC07EDE64u, 0x46EAACCAu,
            0x8DB67F6Fu, 0x90B34FD7u, 0x5BEF9C72u, 0xDD7BEEDCu, 0x16273D79u,
            0xE7718FACu, 0x2C2D5C09u, 0xAAB92EA7u, 0x61E5FD02u, 0x7CE0CDBAu,
            0xB7BC1E1Fu, 0x31286CB1u, 0xFA74BF14u, 0x1EB014D8u, 0xD5ECC77Du,
            0x5378B5D3u, 0x98246676u, 0x852156CEu, 0x4E7D856Bu, 0xC8E9F7C5u,
            0x03B52460u, 0xF2E396B5u, 0x39BF4510u, 0xBF2B37BEu, 0x7477E41Bu,
            0x6972D4A3u, 0xA22E0706u, 0x24BA75A8u, 0xEFE6A60Du, 0x1D661643u,
            0xD63AC5E6u, 0x50AEB748u, 0x9BF264EDu, 0x86F75455u, 0x4DAB87F0u,
            0xCB3FF55Eu, 0x006326FBu, 0xF135942Eu, 0x3A69478Bu, 0xBCFD3525u,
            0x77A1E680u, 0x6AA4D638u, 0xA1F8059Du, 0x276C7733u, 0xEC30A496u,
            0x191C11EEu, 0xD240C24Bu, 0x54D4B0E5u, 0x9F886340u, 0x828D53F8u,
            0x49D1805Du, 0xCF45F2F3u, 0x04192156u, 0xF54F9383u, 0x3E134026u,
            0xB8873288u, 0x73DBE12Du, 0x6EDED195u, 0xA5820230u, 0x2316709Eu,
            0xE84AA33Bu, 0x1ACA1375u, 0xD196C0D0u, 0x5702B27Eu, 0x9C5E61DBu,
            0x815B5163u, 0x4A0782C6u, 0xCC93F068u, 0x07CF23CDu, 0xF6999118u,
            0x3DC542BDu, 0xBB513013u, 0x700DE3B6u, 0x6D08D30Eu, 0xA65400ABu,
            0x20C07205u, 0xEB9CA1A0u, 0x11E81EB4u, 0xDAB4CD11u, 0x5C20BFBFu,
            0x977C6C1Au, 0x8A795CA2u, 0x41258F07u, 0xC7B1FDA9u, 0x0CED2E0Cu,
            0xFDBB9CD9u, 0x36E74F7Cu, 0xB0733DD2u, 0x7B2FEE77u, 0x662ADECFu,
            0xAD760D6Au, 0x2BE27FC4u, 0xE0BEAC61u, 0x123E1C2Fu, 0xD962CF8Au,
            0x5FF6BD24u, 0x94AA6E81u, 0x89AF5E39u, 0x42F38D9Cu, 0xC467FF32u,
            0x0F3B2C97u, 0xFE6D9E42u, 0x35314DE7u, 0xB3A53F49u, 0x78F9ECECu,
            0x65FCDC54u, 0xAEA00FF1u, 0x28347D5Fu, 0xE368AEFAu, 0x16441B82u,
            0xDD18C827u, 0x5B8CBA89u, 0x90D0692Cu, 0x8DD55994u, 0x46898A31u,
            0xC01DF89Fu, 0x0B412B3Au, 0xFA1799EFu, 0x314B4A4Au, 0xB7DF38E4u,
            0x7C83EB41u, 0x6186DBF9u, 0xAADA085Cu, 0x2C4E7AF2u, 0xE712A957u,
            0x15921919u, 0xDECECABCu, 0x585AB812u, 0x93066BB7u, 0x8E035B0Fu,
            0x455F88AAu, 0xC3CBFA04u, 0x089729A1u, 0xF9C19B74u, 0x329D48D1u,
            0xB4093A7Fu, 0x7F55E9DAu, 0x6250D962u, 0xA90C0AC7u, 0x2F987869u,
            0xE4C4ABCCu
        };
        private static readonly uint[] s_crcTable_6 = new uint[256] {
            0x00000000u, 0xA6770BB4u, 0x979F1129u, 0x31E81A9Du, 0xF44F2413u,
            0x52382FA7u, 0x63D0353Au, 0xC5A73E8Eu, 0x33EF4E67u, 0x959845D3u,
            0xA4705F4Eu, 0x020754FAu, 0xC7A06A74u, 0x61D761C0u, 0x503F7B5Du,
            0xF64870E9u, 0x67DE9CCEu, 0xC1A9977Au, 0xF0418DE7u, 0x56368653u,
            0x9391B8DDu, 0x35E6B369u, 0x040EA9F4u, 0xA279A240u, 0x5431D2A9u,
            0xF246D91Du, 0xC3AEC380u, 0x65D9C834u, 0xA07EF6BAu, 0x0609FD0Eu,
            0x37E1E793u, 0x9196EC27u, 0xCFBD399Cu, 0x69CA3228u, 0x582228B5u,
            0xFE552301u, 0x3BF21D8Fu, 0x9D85163Bu, 0xAC6D0CA6u, 0x0A1A0712u,
            0xFC5277FBu, 0x5A257C4Fu, 0x6BCD66D2u, 0xCDBA6D66u, 0x081D53E8u,
            0xAE6A585Cu, 0x9F8242C1u, 0x39F54975u, 0xA863A552u, 0x0E14AEE6u,
            0x3FFCB47Bu, 0x998BBFCFu, 0x5C2C8141u, 0xFA5B8AF5u, 0xCBB39068u,
            0x6DC49BDCu, 0x9B8CEB35u, 0x3DFBE081u, 0x0C13FA1Cu, 0xAA64F1A8u,
            0x6FC3CF26u, 0xC9B4C492u, 0xF85CDE0Fu, 0x5E2BD5BBu, 0x440B7579u,
            0xE27C7ECDu, 0xD3946450u, 0x75E36FE4u, 0xB044516Au, 0x16335ADEu,
            0x27DB4043u, 0x81AC4BF7u, 0x77E43B1Eu, 0xD19330AAu, 0xE07B2A37u,
            0x460C2183u, 0x83AB1F0Du, 0x25DC14B9u, 0x14340E24u, 0xB2430590u,
            0x23D5E9B7u, 0x85A2E203u, 0xB44AF89Eu, 0x123DF32Au, 0xD79ACDA4u,
            0x71EDC610u, 0x4005DC8Du, 0xE672D739u, 0x103AA7D0u, 0xB64DAC64u,
            0x87A5B6F9u, 0x21D2BD4Du, 0xE47583C3u, 0x42028877u, 0x73EA92EAu,
            0xD59D995Eu, 0x8BB64CE5u, 0x2DC14751u, 0x1C295DCCu, 0xBA5E5678u,
            0x7FF968F6u, 0xD98E6342u, 0xE86679DFu, 0x4E11726Bu, 0xB8590282u,
            0x1E2E0936u, 0x2FC613ABu, 0x89B1181Fu, 0x4C162691u, 0xEA612D25u,
            0xDB8937B8u, 0x7DFE3C0Cu, 0xEC68D02Bu, 0x4A1FDB9Fu, 0x7BF7C102u,
            0xDD80CAB6u, 0x1827F438u, 0xBE50FF8Cu, 0x8FB8E511u, 0x29CFEEA5u,
            0xDF879E4Cu, 0x79F095F8u, 0x48188F65u, 0xEE6F84D1u, 0x2BC8BA5Fu,
            0x8DBFB1EBu, 0xBC57AB76u, 0x1A20A0C2u, 0x8816EAF2u, 0x2E61E146u,
            0x1F89FBDBu, 0xB9FEF06Fu, 0x7C59CEE1u, 0xDA2EC555u, 0xEBC6DFC8u,
            0x4DB1D47Cu, 0xBBF9A495u, 0x1D8EAF21u, 0x2C66B5BCu, 0x8A11BE08u,
            0x4FB68086u, 0xE9C18B32u, 0xD82991AFu, 0x7E5E9A1Bu, 0xEFC8763Cu,
            0x49BF7D88u, 0x78576715u, 0xDE206CA1u, 0x1B87522Fu, 0xBDF0599Bu,
            0x8C184306u, 0x2A6F48B2u, 0xDC27385Bu, 0x7A5033EFu, 0x4BB82972u,
            0xEDCF22C6u, 0x28681C48u, 0x8E1F17FCu, 0xBFF70D61u, 0x198006D5u,
            0x47ABD36Eu, 0xE1DCD8DAu, 0xD034C247u, 0x7643C9F3u, 0xB3E4F77Du,
            0x1593FCC9u, 0x247BE654u, 0x820CEDE0u, 0x74449D09u, 0xD23396BDu,
            0xE3DB8C20u, 0x45AC8794u, 0x800BB91Au, 0x267CB2AEu, 0x1794A833u,
            0xB1E3A387u, 0x20754FA0u, 0x86024414u, 0xB7EA5E89u, 0x119D553Du,
            0xD43A6BB3u, 0x724D6007u, 0x43A57A9Au, 0xE5D2712Eu, 0x139A01C7u,
            0xB5ED0A73u, 0x840510EEu, 0x22721B5Au, 0xE7D525D4u, 0x41A22E60u,
            0x704A34FDu, 0xD63D3F49u, 0xCC1D9F8Bu, 0x6A6A943Fu, 0x5B828EA2u,
            0xFDF58516u, 0x3852BB98u, 0x9E25B02Cu, 0xAFCDAAB1u, 0x09BAA105u,
            0xFFF2D1ECu, 0x5985DA58u, 0x686DC0C5u, 0xCE1ACB71u, 0x0BBDF5FFu,
            0xADCAFE4Bu, 0x9C22E4D6u, 0x3A55EF62u, 0xABC30345u, 0x0DB408F1u,
            0x3C5C126Cu, 0x9A2B19D8u, 0x5F8C2756u, 0xF9FB2CE2u, 0xC813367Fu,
            0x6E643DCBu, 0x982C4D22u, 0x3E5B4696u, 0x0FB35C0Bu, 0xA9C457BFu,
            0x6C636931u, 0xCA146285u, 0xFBFC7818u, 0x5D8B73ACu, 0x03A0A617u,
            0xA5D7ADA3u, 0x943FB73Eu, 0x3248BC8Au, 0xF7EF8204u, 0x519889B0u,
            0x6070932Du, 0xC6079899u, 0x304FE870u, 0x9638E3C4u, 0xA7D0F959u,
            0x01A7F2EDu, 0xC400CC63u, 0x6277C7D7u, 0x539FDD4Au, 0xF5E8D6FEu,
            0x647E3AD9u, 0xC209316Du, 0xF3E12BF0u, 0x55962044u, 0x90311ECAu,
            0x3646157Eu, 0x07AE0FE3u, 0xA1D90457u, 0x579174BEu, 0xF1E67F0Au,
            0xC00E6597u, 0x66796E23u, 0xA3DE50ADu, 0x05A95B19u, 0x34414184u,
            0x92364A30u
        };
        private static readonly uint[] s_crcTable_7 = new uint[256] {
            0x00000000u, 0xCCAA009Eu, 0x4225077Du, 0x8E8F07E3u, 0x844A0EFAu,
            0x48E00E64u, 0xC66F0987u, 0x0AC50919u, 0xD3E51BB5u, 0x1F4F1B2Bu,
            0x91C01CC8u, 0x5D6A1C56u, 0x57AF154Fu, 0x9B0515D1u, 0x158A1232u,
            0xD92012ACu, 0x7CBB312Bu, 0xB01131B5u, 0x3E9E3656u, 0xF23436C8u,
            0xF8F13FD1u, 0x345B3F4Fu, 0xBAD438ACu, 0x767E3832u, 0xAF5E2A9Eu,
            0x63F42A00u, 0xED7B2DE3u, 0x21D12D7Du, 0x2B142464u, 0xE7BE24FAu,
            0x69312319u, 0xA59B2387u, 0xF9766256u, 0x35DC62C8u, 0xBB53652Bu,
            0x77F965B5u, 0x7D3C6CACu, 0xB1966C32u, 0x3F196BD1u, 0xF3B36B4Fu,
            0x2A9379E3u, 0xE639797Du, 0x68B67E9Eu, 0xA41C7E00u, 0xAED97719u,
            0x62737787u, 0xECFC7064u, 0x205670FAu, 0x85CD537Du, 0x496753E3u,
            0xC7E85400u, 0x0B42549Eu, 0x01875D87u, 0xCD2D5D19u, 0x43A25AFAu,
            0x8F085A64u, 0x562848C8u, 0x9A824856u, 0x140D4FB5u, 0xD8A74F2Bu,
            0xD2624632u, 0x1EC846ACu, 0x9047414Fu, 0x5CED41D1u, 0x299DC2EDu,
            0xE537C273u, 0x6BB8C590u, 0xA712C50Eu, 0xADD7CC17u, 0x617DCC89u,
            0xEFF2CB6Au, 0x2358CBF4u, 0xFA78D958u, 0x36D2D9C6u, 0xB85DDE25u,
            0x74F7DEBBu, 0x7E32D7A2u, 0xB298D73Cu, 0x3C17D0DFu, 0xF0BDD041u,
            0x5526F3C6u, 0x998CF358u, 0x1703F4BBu, 0xDBA9F425u, 0xD16CFD3Cu,
            0x1DC6FDA2u, 0x9349FA41u, 0x5FE3FADFu, 0x86C3E873u, 0x4A69E8EDu,
            0xC4E6EF0Eu, 0x084CEF90u, 0x0289E689u, 0xCE23E617u, 0x40ACE1F4u,
            0x8C06E16Au, 0xD0EBA0BBu, 0x1C41A025u, 0x92CEA7C6u, 0x5E64A758u,
            0x54A1AE41u, 0x980BAEDFu, 0x1684A93Cu, 0xDA2EA9A2u, 0x030EBB0Eu,
            0xCFA4BB90u, 0x412BBC73u, 0x8D81BCEDu, 0x8744B5F4u, 0x4BEEB56Au,
            0xC561B289u, 0x09CBB217u, 0xAC509190u, 0x60FA910Eu, 0xEE7596EDu,
            0x22DF9673u, 0x281A9F6Au, 0xE4B09FF4u, 0x6A3F9817u, 0xA6959889u,
            0x7FB58A25u, 0xB31F8ABBu, 0x3D908D58u, 0xF13A8DC6u, 0xFBFF84DFu,
            0x37558441u, 0xB9DA83A2u, 0x7570833Cu, 0x533B85DAu, 0x9F918544u,
            0x111E82A7u, 0xDDB48239u, 0xD7718B20u, 0x1BDB8BBEu, 0x95548C5Du,
            0x59FE8CC3u, 0x80DE9E6Fu, 0x4C749EF1u, 0xC2FB9912u, 0x0E51998Cu,
            0x04949095u, 0xC83E900Bu, 0x46B197E8u, 0x8A1B9776u, 0x2F80B4F1u,
            0xE32AB46Fu, 0x6DA5B38Cu, 0xA10FB312u, 0xABCABA0Bu, 0x6760BA95u,
            0xE9EFBD76u, 0x2545BDE8u, 0xFC65AF44u, 0x30CFAFDAu, 0xBE40A839u,
            0x72EAA8A7u, 0x782FA1BEu, 0xB485A120u, 0x3A0AA6C3u, 0xF6A0A65Du,
            0xAA4DE78Cu, 0x66E7E712u, 0xE868E0F1u, 0x24C2E06Fu, 0x2E07E976u,
            0xE2ADE9E8u, 0x6C22EE0Bu, 0xA088EE95u, 0x79A8FC39u, 0xB502FCA7u,
            0x3B8DFB44u, 0xF727FBDAu, 0xFDE2F2C3u, 0x3148F25Du, 0xBFC7F5BEu,
            0x736DF520u, 0xD6F6D6A7u, 0x1A5CD639u, 0x94D3D1DAu, 0x5879D144u,
            0x52BCD85Du, 0x9E16D8C3u, 0x1099DF20u, 0xDC33DFBEu, 0x0513CD12u,
            0xC9B9CD8Cu, 0x4736CA6Fu, 0x8B9CCAF1u, 0x8159C3E8u, 0x4DF3C376u,
            0xC37CC495u, 0x0FD6C40Bu, 0x7AA64737u, 0xB60C47A9u, 0x3883404Au,
            0xF42940D4u, 0xFEEC49CDu, 0x32464953u, 0xBCC94EB0u, 0x70634E2Eu,
            0xA9435C82u, 0x65E95C1Cu, 0xEB665BFFu, 0x27CC5B61u, 0x2D095278u,
            0xE1A352E6u, 0x6F2C5505u, 0xA386559Bu, 0x061D761Cu, 0xCAB77682u,
            0x44387161u, 0x889271FFu, 0x825778E6u, 0x4EFD7878u, 0xC0727F9Bu,
            0x0CD87F05u, 0xD5F86DA9u, 0x19526D37u, 0x97DD6AD4u, 0x5B776A4Au,
            0x51B26353u, 0x9D1863CDu, 0x1397642Eu, 0xDF3D64B0u, 0x83D02561u,
            0x4F7A25FFu, 0xC1F5221Cu, 0x0D5F2282u, 0x079A2B9Bu, 0xCB302B05u,
            0x45BF2CE6u, 0x89152C78u, 0x50353ED4u, 0x9C9F3E4Au, 0x121039A9u,
            0xDEBA3937u, 0xD47F302Eu, 0x18D530B0u, 0x965A3753u, 0x5AF037CDu,
            0xFF6B144Au, 0x33C114D4u, 0xBD4E1337u, 0x71E413A9u, 0x7B211AB0u,
            0xB78B1A2Eu, 0x39041DCDu, 0xF5AE1D53u, 0x2C8E0FFFu, 0xE0240F61u,
            0x6EAB0882u, 0xA201081Cu, 0xA8C40105u, 0x646E019Bu, 0xEAE10678u,
            0x264B06E6u
        };

        private static uint ManagedCrc32(uint crc32, byte[] buffer, int offset, int length)
        {
            Debug.Assert(BitConverter.IsLittleEndian, "ManagedCrc32 Expects Little Endian");

            uint term1, term2, term3 = 0;

            crc32 ^= 0xFFFFFFFFU;
            int runningLength = (length / 8) * 8;
            int endBytes = length - runningLength;

            for (int i = 0; i < runningLength / 8; i++)
            {
                crc32 ^= (uint)(buffer[offset] | buffer[offset + 1] << 8 | buffer[offset + 2] << 16 | buffer[offset + 3] << 24);
                offset += 4;
                term1 = s_crcTable_7[crc32 & 0x000000FF] ^
                        s_crcTable_6[(crc32 >> 8) & 0x000000FF];
                term2 = crc32 >> 16;
                crc32 = term1 ^
                        s_crcTable_5[term2 & 0x000000FF] ^
                        s_crcTable_4[(term2 >> 8) & 0x000000FF];


                term3 = (uint)(buffer[offset] | buffer[offset + 1] << 8 | buffer[offset + 2] << 16 | buffer[offset + 3] << 24);
                offset += 4;
                term1 = s_crcTable_3[term3 & 0x000000FF] ^
                        s_crcTable_2[(term3 >> 8) & 0x000000FF];
                term2 = term3 >> 16;
                crc32 ^= term1 ^
                        s_crcTable_1[term2 & 0x000000FF] ^
                        s_crcTable_0[(term2 >> 8) & 0x000000FF];
            }

            for (int i = 0; i < endBytes; i++)
            {
                crc32 = s_crcTable_0[(crc32 ^ buffer[offset++]) & 0x000000FF] ^ (crc32 >> 8);
            }

            crc32 ^= 0xFFFFFFFFU;
            return crc32;
        }
#endif
    }
}
