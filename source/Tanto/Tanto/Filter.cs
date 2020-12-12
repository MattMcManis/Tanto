/* ----------------------------------------------------------------------
Tanto
Copyright (C) 2018-2020 Matt McManis
http://github.com/MattMcManis/Tanto
mattmcmanis@outlook.com

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see <http://www.gnu.org/licenses/>. 
---------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Tanto
{
    public partial class Filter
    {
        /// <summary>
        ///    Filter File Name (Method)
        /// </summary>
        /// <remarks>
        ///    This filter is applied to Original File Name.
        /// </remarks>
        public static String FilterFileName(MainWindow mainwindow, string filename)
        {
            // -------------------------
            // Keep Original Spacing (Separate)
            // -------------------------
            // Example.01.The.Episode.Name.(1080p).Blu-Ray.mp4
            // to
            // Example . 01 . The . Episode . Name . (1080p) . Blu-Ray .mp4
            if (mainwindow.cbxFilterOriginalSpacing.IsChecked == true)
            {
                if (!string.IsNullOrWhiteSpace(mainwindow.tbxSpacing.Text))
                {
                    try
                    {
                        string character = @"\" + mainwindow.tbxSpacing.Text;
                        filename = Regex.Replace(filename, character, " " + mainwindow.tbxSpacing.Text + " ");
                    }
                    catch
                    {
                        MessageBox.Show("Could not replace spacing.",
                                        "Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }

            // -------------------------
            // Remove Period Spacing
            // -------------------------
            if (mainwindow.cbxFilterPeriodSpacing.IsChecked == true)
            {
                try
                {
                    // replace with space
                    string regex = @"(?<!\.)\.(?!\.)";
                    filename = Regex.Replace(filename, regex, " ");
                }
                catch
                {
                    MessageBox.Show("Could not remove periods.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // -------------------------
            // Remove Underscore Spacing
            // -------------------------
            if (mainwindow.cbxFilterUnderscoreSpacing.IsChecked == true)
            {
                // replace with space
                filename = Regex.Replace(filename, "_", " ");
            }

            // -------------------------
            // Remove Dash Spacing
            // -------------------------
            if (mainwindow.cbxFilterDashSpacing.IsChecked == true
                && mainwindow.cbxFilterHyphens.IsChecked == false)
            {
                // replace with space
                filename = Regex.Replace(filename, "-", " "); 
            }

            // -------------------------
            // Remove All Dashes
            // -------------------------
            if (mainwindow.cbxFilterDashes.IsChecked == true
                && mainwindow.cbxFilterHyphens.IsChecked == false)
            {
                // remove dash
                filename = Regex.Replace(filename, "-", "").Trim();
            }
            // -------------------------
            // Remove Dashes Between - Words
            // Preserve Hyphenated-Words
            // -------------------------
            else if (mainwindow.cbxFilterDashes.IsChecked == true
                && mainwindow.cbxFilterHyphens.IsChecked == true)
            {
                // remove dash, preserve hyphens
                filename = Regex.Replace(filename, @"\b(-+)\b|-", "$1").Trim();
                // multiple dashes to single dash
                filename = Regex.Replace(filename, "[-]{2,}", "").Trim();
            }

            // -------------------------
            // Remove Year
            // -------------------------
            if (mainwindow.cbxFilterRemoveYear.IsChecked == true)
            {
                filename = Regex.Replace(filename, @"(\((1|2)\d?\d?\d?\))", "");
            }

            // -------------------------
            // Remove Tags
            // -------------------------
            if (mainwindow.cbxFilterRemoveTags.IsChecked == true)
            {
                //filename = Regex.Replace(
                //filename
                //    , @"(?i)(\[).*?(\])|(\(*)(\d+(mb|MB)|480p|576p|720p|1080p|2k|4k|8k|60fps|DVD|BD|BRD|Bluray|Blu-Ray|BrRip|WebDL|Web-DL|WebRip|Web-Rip|RAW|HEVC|(h|x)(265|264)|AV1|VP8|VP9|Theora|AAC|AC3|Vorbis|Opus|FLAC|MP3|(DTS\s?|DD\s?)(5\.1|7\.1)(\s?CH)?|(2|6|5\.1|7\.1)\s?CH|Dual-Audio|Multi-Sub|English Dub)(\)*)(\,*)"
                //    , ""
                //    , RegexOptions.IgnoreCase
                //);

                // HW Accel Transcode
                string hwAccelTranscode = "(NVIDIA NVENC|Intel QSV|AMD AMF)";

                // Presets
                IEnumerable<string> presetsList = new List<string>()
                {
                    "placebo",
                    "very-slow",
                    "slower",
                    "slow",
                    "medium",
                    "fast",
                    "faster",
                    "very-fast",
                    "super-fast",
                    "ultra-fast"
                };
                string presets = @"(p\-(" + string.Join("|", presetsList) + @"))";

                // Pass
                string pass = @"((1|2)\-?\s?Pass)";

                // Sample Rate
                string sampleRate = @"(\d+\.?\d?kHz)";

                // Bit Rate
                IEnumerable<string> bitRateList = new List<string>()
                {
                    @"CRF\d+",
                    @"\d+(\.?\d+)?(kVBR|kbps|k|m)",
                };
                string bitRate = "(" + string.Join("|", bitRateList) + ")";

                // Pixel Format
                string pixelFormat = "(yuvj444p|yuvj422p|yuvj420p|yuva444p9le|yuva444p16le|yuva444p10le|yuva444p|yuva422p9le|yuva422p16le|yuva422p10le|yuva422p|yuva420p9le|yuva420p16le|yuva420p10le|yuva420p|yuv444p9le|yuv444p16le|yuv444p14le|yuv444p12le|yuv444p10le|yuv444p|yuv440p12le|yuv440p10le|yuv440p|yuv422p9le|yuv422p16le|yuv422p14le|yuv422p12le|yuv422p10le|yuv422p|yuv420p9le|yuv420p16le|yuv420p14le|yuv420p12le|yuv420p10le|yuv420p|yuv411p|yuv410p|ya8|ya16be|rgba64le|rgba64be|rgba|rgb48le|rgb48be|rgb24|pal8|nv21|nv20le|nv16|nv12|monob|gray9le|gray16le|gray16be|gray12le|gray10le|gray|gbrp9le|gbrp16le|gbrp14le|gbrp12le|gbrp10le|gbrp|gbrap16le|gbrap12le|gbrap10le|gbrap|bgra|bgr0)";

                // Profile
                // (e.g. Hi444PP, Hi10P)
                string profile = @"(Hi(\d+)(P|PP))";

                // Size
                IEnumerable<string> sizeList = new List<string>()
                {
                    @"\d+p", //1080p
                    @"(8|4|2)\s?K(\s?UHD)?", //4K UHD
                    @"\d\d\d\d?x\d\d\d\d?", //1920x1080, 720x480
                };
                string size = "(" + string.Join("|", sizeList) + ")";

                // Scaling Algorithm
                string scaling = "(spline|sinc|neighbor|lanczos|gauss|fast_bilinear|experimental|bilinear|bicublin|bicubic|area)";

                // FPS
                // (e.g. 23.976fps, 60fps)
                string fps = @"(\d+.?\d+?\s?fps)";

                // Subtitles
                string langs = @"(Eng(lish)?|E|Ara(bic)?|Ben(gali)?|Chi(nese)?|Chn|Dut(ch)?|Fin(nish)?|Fre(nch)?|Ger(man)?|De|Hin(di)?|Ita(lian)?|Jap(anese)?|Kor(ean)?|Man(darin)?|Por(tuguese)?|Rus(sian)?|Spa(nish)?|Swe(dish)?|Tel(ugu)?|Vie(tnamese)?)";
                string subs = @"(Subtitle(s|d)?|Sub(s)?)";
                string subtitles1 = @"(" + langs + @"[\-\s]?" + subs + @")";
                string subtitles2 = @"|(" + subs + @"[\-\s]?" + langs + @")";
                string subtitles3 = @"|(" + langs.Replace("|E", "").Replace("|Ben(gali)?", "|Bengali").Replace("Chi(nese)?", "Chinese").Replace("|Man(darin)?", "|Mandarin") + ")"; // Fix words like Ben, Chi, Man
                //string subtitles4 = @"|\b" + subs + @"\b";
                string subtitles4 = @"|(Multi[\-\s]?Sub(s)?|Subtitle(s|d)?|Sub(s|bed)?)";
                string subtitles = subtitles1 +
                                   subtitles2 +
                                   subtitles3 +
                                   subtitles4;

                // Channel
                string channel1 = @"(\d+(?:\.\d+)?[.\-_\s]?)?(CH)\s?(?(1)|([.\s]?\d+(?:\.\d+)?)?)";
                string channel2 = @"|(\d+(?:\.\d+)?[.-_\s]?)?((Dolby[.\-_\s]?(Digital)?)[.\-_\s]?(?:Pro[.\-_\s]?(Logic)?[.\-_\s]?(II)?|Surround|Atmos|TrueHD|Vision)?)[.\-_\s]?(?(1)|([.\s]?\d+(?:\.\d+)?)?)";
                string channel3 = @"|(\d+(?:\.\d+)?[.\-_\s]?)?(AC3|AAC|DTS|(DD(?:P|\+?)))(?(1)|([.\-_\s]?\d+(?:\.\d+)?)?)";
                string channel4 = @"|(2\.0|2\.1|3\.1|5\.1|7\.1|7\.1\.2|7\.2|9\.1|9\.1\.2)"; // standalone
                string channel = channel1 +
                                 channel2 +
                                 channel3 +
                                 channel4;

                // Bit Depth
                string bitDepth = @"(\d+[\-\s]?bit)";

                // Tags
                string tags = @"(\[.*?\])"; // [tag] // do not wrap with \b

                // Formats
                IEnumerable<string> formatsList = new List<string>()
                {
                    @"(DVD|BRD|BD|Br|HD|SD|Web)[\-\s]?(Rip|DL)?",
                    @"(HD|SD)(TV|R|C)?",
                    @"Blu[\-\s]?Ray",
                    //@"Web[\-\s]?DL",
                    @"Rip",
                    @"(\d+)?CD",
                    @"Playlist",
                };
                string formats = "(" + string.Join("|", formatsList) + ")";

                // Containers
                string containers = @"(yuv|wv|wmv|wma|webm|wav|vox|vob|ts|swf|svi|sln|rmvb|rm|raw|ra|qt|pnm|pdf|ogv|ogg|oga|nsv|mxf|msv|mpv|mpg|mpeg|mpe|mpc|mp4|mov|mod|mng|mmf|mkv|m4v|m4p|m4b|m4a|m2v|jpg|jpeg|jfif|ivs|iklax|heif|gifv|flv|f4v|f4p|f4b|f4a|exif|eps|dvf|dss|drc|dct|crw|cr2|awb|avi|au|asf|amr|aiff|act|aax|aa|8svx|3gp|3g2)|(RAW|Lossless|HEVC|H[.\-]?(264|265)|(x264|x265)[.\-\s]?QOQ|QOQ|NF|FP|x265|x264|012v|zmbv|zlib|zerocodec|yuv4|yop|ylc|y41p|xwd|xvid|xsub|xpm|xma2|xma1|xface|xbm|xbin|xan_wc4|xan_wc3|xan_dpcm|ws_vqa|wrapped_avframe|wnv1|wmv3image|wmv3|wmv2|wmv1|wmavoice|wmav2|wmav1|wmapro|wmalossless|westwood_snd1|webvtt|webp|wcmv|vp9|vp8|vp7|vp6f|vp6a|vp6|vp5|vp4|vp3|vorbis|vmnc|vmdvideo|vmdaudio|vixl|vcr1|vc1image|vc1|vble|vb|v410|v408|v308|v210x|v210|utvideo|ulti|txd|twinvq|ttml|ttf|tta|tscc2|tscc|truemotion2rt|truemotion2|truemotion1|truehd|tqi|tmv|timed_id3|tiff|tiertexseqvideo|thp|theora|tgv|tgq|text|tdsc|targa_y216|targa|tak|svq3|svq1|svg|sunrast|subviewer1|subviewer|subrip|stl|ssa|srt|srgc|speedhq|sp5x|sonicls|sol_dpcm|smvjpeg|smv|smc|smackvideo|smackaudio|siren|sipr|shorten|sheervideo|sgirle|sgi|sdx2_dpcm|scte_35|screenpresso|scpr|sbc|sanm|sami|s302m|rv40|rv30|rv20|rv10|rscc|rpza|roq_dpcm|roq|rl2|realtext|rawvideo|rasc|ralf|ra_288|ra_144|r210|r10k|qtrle|qpeg|qdraw|qdmc|qdm2|qcelp|ptx|psd|prosumer|prores|ppm|png|pjs|pixlet|pictor|pgmyuv|pgm|pcx|pcm_vidc|pcm_u8|pcm_u32le|pcm_u32be|pcm_u24le|pcm_u24be|pcm_u16le|pcm_u16be|pcm_s8_planar|pcm_s8|pcm_s64le|pcm_s64be|pcm_s32le_planar|pcm_s32le|pcm_s32be|pcm_s24le_planar|pcm_s24le|pcm_s24daud|pcm_s24be|pcm_s16le_planar|pcm_s16le|pcm_s16be_planar|pcm_s16be|pcm_mulaw|pcm_lxf|pcm_f64le|pcm_f64be|pcm_f32le|pcm_f32be|pcm_f24le|pcm_f16le|pcm_dvd|pcm_bluray|pcm_alaw|pcm|pbm|pam|paf_video|paf_audio|otf|opus|nuv|notchlc|nellymoser|mxpeg|mwsc|mvha|mvdv|mvc2|mvc1|mv30|musepack8|musepack7|mts2|mszh|msvideo1|mss2|mss1|msrle|msmpeg4v3|msmpeg4v2|msmpeg4v1|mscc|msa1|mpl2|mpegh_3d_audio|mpeg4|mpeg2video|mpeg1video|mp4als|mp3on4|mp3adu|mp3|mp2|mp1|mov_text|motionpixels|mmvideo|mlp|mjpegb|mjpeg|mimic|microdvd|metasound|mdec|magicyuv|mad|mace6|mace3|m101|lscr|loco|ljpeg|libmp3lame|lame|lagarith|kmvc|klv|kgv1|jv|jpegls|jpeg2000|jacosub|interplayvideo|interplayacm|interplay_dpcm|indeo5|indeo4|indeo3|indeo2|imm5|imm4|imc|ilbc|iff_ilbm|idf|idcin|iac|hymt|huffyuv|hqx|hq_hqa|hnm4video|hevc|hdmv_text_subtitle|hdmv_pgs_subtitle|hcom|hca|hap|h265|h264|h263p|h263i|h263|h261|gsm_ms|gsm|gremlin_dpcm|gif|gdv|g729|g723_1|g2m|frwu|fraps|fmvc|flv1|flic|flashsv2|flashsv|flac|fits|fic|ffvhuff|ffv1|exr|evrc|escape130|escape124|epg|eia_608|eac3|dxv|dxtory|dxa|dvvideo|dvd_subtitle|dvd_nav_packet|dvb_teletext|dvb_subtitle|dvaudio|dts|dst|dss_sp|dsicinvideo|dsicinaudio|dsd_msbf_planar|dsd_msbf|dsd_lsbf_planar|dsd_lsbf|dpx|dolby_e|dnxhd|dirac|dfa|derf_dpcm|dds|daala|cyuv|cscd|cpia|cook|comfortnoise|codec2|cmv|cllc|cljr|clearvideo|cinepak|cfhd|celt|cdxl|cdtoons|cdgraphics|cavs|c93|brender_pix|bmv_video|bmv_audio|bmp|bitpacked|bintext|binkvideo|binkaudio_rdft|binkaudio_dct|bin_data|bfi|bethsoftvid|ayuv|avui|avs2|avs|avrp|avrn|avc|av1|aura2|aura|atrac9|atrac3pal|atrac3p|atrac3al|atrac3|atrac1|asv2|asv1|ass|arib_caption|arbc|aptx_hd|aptx|apng|ape|ansi|anm|amv|amr_wb|amr_nb|alias_pix|alac|aic|agm|adpcm_zork|adpcm_yamaha|adpcm_xa|adpcm_vima|adpcm_thp_le|adpcm_thp|adpcm_swf|adpcm_sbpro_4|adpcm_sbpro_3|adpcm_sbpro_2|adpcm_psx|adpcm_mtaf|adpcm_ms|adpcm_ima_ws|adpcm_ima_wav|adpcm_ima_ssi|adpcm_ima_smjpeg|adpcm_ima_rad|adpcm_ima_qt|adpcm_ima_oki|adpcm_ima_mtf|adpcm_ima_iss|adpcm_ima_ea_sead|adpcm_ima_ea_eacs|adpcm_ima_dk4|adpcm_ima_dk3|adpcm_ima_dat4|adpcm_ima_cunning|adpcm_ima_apm|adpcm_ima_apc|adpcm_ima_amv|adpcm_ima_alp|adpcm_g726le|adpcm_g726|adpcm_g722|adpcm_ea_xas|adpcm_ea_r3|adpcm_ea_r2|adpcm_ea_r1|adpcm_ea_maxis_xa|adpcm_ea|adpcm_dtk|adpcm_ct|adpcm_argo|adpcm_aica|adpcm_agm|adpcm_afc|adpcm_adx|adpcm_4xm|acelp\.kelvin|ac3|aasc|aac_latm|aac|a64_multi5|a64_multi|8svx_fib|8svx_exp|8bps|4xm|4gv)";

                // Codecs
                List<string> codecsList = new List<string>()
                {
                    "RAW",
                    "Lossless",
                    "HEVC",
                    @"H[.\-]?(264|265)",
                    @"(x264|x265)[.\-\s]?QOQ",
                    "QOQ",
                    "NF",
                    "FP",
                };
                string codecs = "(" + string.Join("|", codecsList) + ")";

                // Video
                //string video = @"(UHD|\d+[\-\s]?bit)";
                IEnumerable<string> videoList = new List<string>()
                {
                    "UHD",
                    "HD",
                    "SD",
                    @"\d+[\-\s]?bit",
                };
                string video = "(" + string.Join("|", videoList) + ")";

                // Audio
                IEnumerable<string> audioList = new List<string>()
                {
                    @"(Dual|Multi|Original|Org)[\-\s]?(Audio|Aud)",
                    langs + @"[\-\s]?Dub",
                    @"(Non[\-\s])?English[\-\s]?Translated",
                    @"Dub(bed)?",
                };
                string audio = "(" + string.Join("|", audioList) + ")";

                // File
                string file = @"(\d+([.]?\d+?)?[.\-_\s]?(MB|GB|TB))"; //100.5MB, 100GB, 100TB

                // Labels
                IEnumerable<string> labelsList = new List<string>()
                {
                    "Amazon",
                    "AMZN",
                    "iTunes",
                    "Spotify",
                    "Repack",
                    "Complete",
                };
                string labels = "(" + string.Join("|", labelsList) + ")";

                // Symbols
                // Stray Parentheses
                string symbols = @"\(\)|\(\s+\)"; // do not wrap with \b


                // Build regex rules
                // Order is important
                IEnumerable<string> regexTagsList = new List<string>()
                {
                    tags,
                    @"((?<=_)|\b)(", // opening
                    hwAccelTranscode,
                    sampleRate,
                    bitRate,
                    channel,
                    pixelFormat,
                    formats,
                    containers,
                    codecs,
                    @"(cv-copy)",
                    @"(ca-copy)",
                    video,
                    pass,
                    presets,
                    profile,
                    size,
                    @"(sz-source)",
                    scaling,
                    fps,
                    subtitles,
                    audio,
                    bitDepth,
                    file,
                    labels,
                    @")((?=_)|\b)", //closing
                    symbols,
                };

                string regexTags = string.Join("|", regexTagsList
                                             // don't re-order
                                             .Distinct()
                                       )
                                       // Remove invalid rules caused by adding "((?<=_)|\b)(" to the list
                                       .Replace("(|", "(")
                                       .Replace("|)", ")");

                // Remove Tags and Tokens
                filename = Regex.Replace(
                    filename,
                    regexTags,
                    "",
                    RegexOptions.IgnoreCase
                );
            }

            // -------------------------
            // Remove Double Spaces
            // -------------------------
            // Use at the very end of all filters
            if (mainwindow.cbxFilterDoubleSpaces.IsChecked == true)
            {
                filename = Regex.Replace(filename, @"\s+", " ");
            }


            return filename;
        }

        /// <summary>
        ///    Filter Episode Name (Method)
        /// </summary>
        public static String FilterEpisodeName(MainWindow mainwindow, string filename)
        {
            // -------------------------
            // Episode Name Spacing
            // -------------------------
            // Separate title string with no spaces into words
            // e.g. ThisIsATitle -> This Is A Title
            if (mainwindow.cbxFilterEpisodeNameSpacing.IsChecked == true)
            {
                Regex regex = new Regex("(?<!^|[\\sA-Z\\p{P}])[A-Z]|(?<=\\p{P})\\p{P}|(?<=[a-z])(?=\\d)|(?<=\\d)(?=[a-z])|(?<=[A-Z])(?=\\d)|(?<=\\d)(?=[A-Z])|(((?<!^)(?<![\\s'([{])[A-Z](?=[a-z]))|((?<=[a-z])[A-Z]))|(?<!^)(?=[[({&])|(?<=[)\\]}!&}])", RegexOptions.Multiline);
                filename = regex.Replace(filename, @" $0");
            }

            // -------------------------
            // Title Case
            // -------------------------
            // e.g. this is an episode name -> This Is An Episode Name
            if (mainwindow.cbxFilterTitleCase.IsChecked == true)
            {
                string input = filename;
                int minLength = 2;
                string regexPattern = string.Format(@"(?<=(^|[.!?])\s*)\w|\b\w(?=[-\w]{{{0}}})", minLength);
                filename = Regex.Replace(input, regexPattern, m => m.Value.ToUpperInvariant());
            }

            return filename;
        }

        /// <summary>
        ///    Filter Remove (Method)
        /// </summary>
        /// <remarks>
        ///    This filter is applied to Episode Name.
        /// </remarks>
        public static String FilterRemove(MainWindow mainwindow, string filename)
        {
            // -------------------------
            // Remove Episode Numbering
            // -------------------------
            if (mainwindow.cbxFilterRemoveEpisodeNumbering.IsChecked == true)
            {
                // S000E000, EP000, E000, 000x000, 000v000, 000, 000v0-9
                filename = Regex.Replace(
                                filename
                                , @"(?i)\b((S\d+\-?((E|EP)?\-?\d+\-?)+)|((E\d+)+)|((E\d+)-(\d+-?)+)|((E\d+)-(E\d+-?)+)|(Episode\s?\d+?)|(EP|EP((\s*)|-))\d+|E\d+|(?<![.])^(\d+-?)+|(^E(\s*)|EP(\s*))?(?<![.])\d+(x|v)\d+)\b"
                                , ""
                                , RegexOptions.IgnoreCase
                                );
            }

            // -------------------------
            // Remove Words
            // -------------------------
            if (!string.IsNullOrWhiteSpace(mainwindow.tbxRemoveWords.Text))
            {
                try
                {
                    string words = mainwindow.tbxRemoveWords.Text.Replace(",", "|").Replace(" ", "");

                    string regex = @"\b(" + words + @")\b";

                    filename = Regex.Replace(filename, regex, "");
                }
                catch
                {
                    MessageBox.Show("Could not remove words.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // -------------------------
            // Remove Characters
            // -------------------------
            if (!string.IsNullOrWhiteSpace(mainwindow.tbxRemoveChars.Text))
            {
                try
                {
                    List<string> removeChars = new List<string>(mainwindow.tbxRemoveChars.Text.Split(','));

                    List<string> removeCharsEscaped = new List<string>();

                    // Escape special characters in list
                    foreach (string character in removeChars)
                    {
                        removeCharsEscaped.Add(Regex.Escape(character.Replace(" ", "")));
                    }

                    // Separate with |
                    string regex = string.Join("|", removeCharsEscaped
                                        .Where(s => !string.IsNullOrEmpty(s))
                                        .Where(s => !s.Equals("\n"))
                                        .Where(s => !s.Equals("\r\n"))
                                        );

                    filename = Regex.Replace(filename, regex, ""); // do not Regex.Escape()
                }
                catch
                {
                    MessageBox.Show("Could not remove characters.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // -------------------------
            // Remove Between Characters
            // -------------------------
            //if (mainwindow.tbxRemoveRangeStart.Text != string.Empty
            //    && mainwindow.tbxRemoveRangeEnd.Text != string.Empty)
            if (!string.IsNullOrWhiteSpace(mainwindow.tbxRemoveRangeStart.Text) &&
                !string.IsNullOrWhiteSpace(mainwindow.tbxRemoveRangeEnd.Text))
            {
                try
                {
                    string start = mainwindow.tbxRemoveRangeStart.Text;
                    string end = mainwindow.tbxRemoveRangeEnd.Text;

                    string regex = "(\\" + start + ".*\\" + end + ")";

                    filename = Regex.Replace(filename, regex, ""); // do not Regex.Escape()
                }
                catch
                {
                    MessageBox.Show("Could not remove between characters.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // -------------------------
            // Remove Double Spaces
            // -------------------------
            // Use at the very end of all filters
            // Also in File Name Filter
            if (mainwindow.cbxFilterDoubleSpaces.IsChecked == true)
            {
                filename = Regex.Replace(filename, @"\s+", " ");
            }

            filename = filename.Trim();

            return filename;
        }


        /// <summary>
        ///    Filter Replace (Method)
        /// </summary>
        /// <remarks>
        ///    This filter is applied to New File Name.
        /// </remarks>
        public static String FilterReplace(MainWindow mainwindow, string filename)
        {
            // Replace These Characters With
            if (!string.IsNullOrWhiteSpace(mainwindow.tbxReplaceChars.Text))
            {
                try
                {
                    string characters = mainwindow.tbxReplaceChars.Text.Replace(",", "|").Replace(" ", "");
                    string replacement = mainwindow.tbxReplaceCharsWith.Text;

                    string regex = characters;

                    filename = Regex.Replace(filename, Regex.Escape(regex), Regex.Escape(replacement));
                }
                catch
                {
                    MessageBox.Show("Could not replace characters.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // Replace These Words With
            if (!string.IsNullOrWhiteSpace(mainwindow.tbxReplaceWords.Text))
            {
                try
                {
                    string words = mainwindow.tbxReplaceWords.Text.Replace(",", "|").Replace(" ", "");
                    string replacement = mainwindow.tbxReplaceWordsWith.Text;

                    string regex = @"\b(" + words + @")\b";

                    filename = Regex.Replace(filename, regex, replacement);
                }
                catch
                {
                    MessageBox.Show("Could not replace words.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // Replace Spaces With
            if (!string.IsNullOrWhiteSpace(mainwindow.tbxReplaceSpacesWith.Text))
            {
                try
                {
                    string replacement = mainwindow.tbxReplaceSpacesWith.Text;

                    filename = Regex.Replace(filename, " ", Regex.Escape(replacement));
                }
                catch
                {
                    MessageBox.Show("Could not replace spaces.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            filename = filename.Trim();

            return filename;
        }


        /// <summary>
        ///    Serites Title Filter (Method)
        /// </summary>
        public static String SeriesTitleFilter(MainWindow mainwindow, string filename)
        {
            //MessageBox.Show(filename); //debug

            // -------------------------
            // Keep Original Spacing (Separate)
            // -------------------------
            // Example.01.The.Episode.Name.(1080p).Blu-Ray.mp4
            // to
            // Example . 01 . The . Episode . Name . (1080p) . Blu-Ray .mp4
            if (mainwindow.cbxFilterOriginalSpacing.IsChecked == true)
            {
                if (!string.IsNullOrWhiteSpace(mainwindow.tbxSpacing.Text))
                {
                    try
                    {
                        string character = @"\" + mainwindow.tbxSpacing.Text; // to remove
                        filename = Regex.Replace(filename, character, " " + mainwindow.tbxSpacing.Text + " ");
                    }
                    catch
                    {
                        MessageBox.Show("Could not replace spacing.",
                                        "Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }

            //MessageBox.Show(filename); //debug

            // -------------------------
            // Remove Year
            // -------------------------
            filename = Regex.Replace(filename, @"\((\d\d\d\d)\)", "");

            // -------------------------
            // Remove Strange Episode Abbreviations
            // -------------------------
            filename = Regex.Replace(filename, @"(?!\.)", "");
            filename = Regex.Replace(filename, @"\b(Ep\.)\B", "", RegexOptions.IgnoreCase);

            // -------------------------
            // Remove Spacing
            // -------------------------
            //if (mainwindow.cbxFilterOriginalSpacing.IsChecked == false)
            //{
                // Period Spacing
                filename = Regex.Replace(filename, @"(?<!\.)\.(?!\.)", " ");

                // Underscore Spacing
                filename = Regex.Replace(filename, "_", " ");
            //}

            // -------------------------
            // Remove Dash, Preserve Hyphens
            // -------------------------
            filename = Regex.Replace(filename, @"((?<=[\s\.])\-+)|(\-+(?=[\s\.]))", "").Trim();
            // Multiple Dashes to Single Dash
            filename = Regex.Replace(filename, "[-]{2,}", "").Trim();

            // -------------------------
            // Remove Double Space
            // -------------------------
            filename = Regex.Replace(filename, @"\s+", " ");

            filename = filename.Trim();


            return filename;
        }


        /// <summary>
        ///    Finalize Filter (Method)
        /// </summary>
        /// <remarks>
        ///    Applied to the New File Name.
        /// </remarks>
        public static String FilterFinalize(MainWindow mainwindow, string filename)
        {
            // -------------------------
            // Keep Original Spacing (Put back together)
            // -------------------------
            if (!string.IsNullOrWhiteSpace(mainwindow.tbxSpacing.Text))
            {
                try
                {
                    string character = @"\" + mainwindow.tbxSpacing.Text;

                    // Add period where missing between items
                    // Example . S01E02 Episode . Name
                    // Example . S01E02 . Episode . Name
                    // Example.S01E02.Episode.Name (Finalized)
                    //filename = Regex.Replace(filename, @"(?<!\.) (?!\.)", " " + character + " ");
                    filename = Regex.Replace(filename, @"(?<!" + character + ") (?!" + character + ")", " " + mainwindow.tbxSpacing.Text + " ");

                    filename = Regex.Replace(filename, @"( " + character + " )", mainwindow.tbxSpacing.Text);
                }
                catch
                {
                    MessageBox.Show("Could not finalize.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

            // Remove Spaces
            if (mainwindow.cbxFilterOriginalSpacing.IsChecked == true)
            {
                filename = Regex.Replace(filename, @"( )", "");
            }

            filename = filename.Trim();

            return filename;
        }

     }
}
