﻿/*
 * YOGEME.exe, All-in-one Mission Editor for the X-wing series, TIE through XWA
 * Copyright (C) 2007-2017 Michael Gaisser (mjgaisser@gmail.com)
 * Licensed under the MPL v2.0 or later
 * 
 * VERSION: 1.3
 */

/* CHANGELOG
 * v1.3, 170107
 * [FIX] Recent mission fixes [JB]
 * v1.2.3, 141214
 * [UPD] change to MPL
 * v1.2.2, 121022
 * [FIX] Mission files opened from command line or "Open with" will no longer show StartForm
 * v1.2, 121006
 * - Settings initialized here, passed on
 * [NEW] RecentMission functionality
 * v1.0, 110921
 * - Release
 */

using System;
using System.IO;
using System.Windows.Forms;

namespace Idmr.Yogeme
{
	static class Program
	{
		[STAThread]
		static void Main(string[] Args)
		{
			Settings config = new Settings();
			if (Args.Length != 1 && config.Startup == Settings.StartupMode.Normal) Application.Run(new StartForm(config));
			else if (Args.Length != 1 && config.Startup == Settings.StartupMode.LastPlatform)
			{
				switch (config.LastPlatform)	//open the last platform directly
				{
					case Settings.Platform.TIE:
						new TieForm(config).Show();
						break;
					case Settings.Platform.XvT:
						new XvtForm(config, false).Show();
						break;
					case Settings.Platform.BoP:
						new XvtForm(config, true).Show(); ;
						break;
					case Settings.Platform.XWA:
						new XwaForm(config).Show();
						break;
				}
			}
			else if (Args.Length != 1 && config.Startup == Settings.StartupMode.LastMission)	//open the last mission directly
			{
				if (config.LastMission != "")
				{
					switch (config.LastPlatform)
					{
						case Settings.Platform.TIE:
							new TieForm(config, config.LastMission).Show();
							break;
						case Settings.Platform.XvT:
							new XvtForm(config, config.LastMission).Show();
							break;
						case Settings.Platform.BoP:
							new XvtForm(config, config.LastMission).Show();
							break;
						case Settings.Platform.XWA:
							new XwaForm(config, config.LastMission).Show();
							break;
					}
				}
				else if (config.RecentMissions[1] != "")  //JB Fixed.  [0] is the currently loaded mission, [1..5] are the Recent missions.
				{
					switch (config.RecentPlatforms[1])
					{
						case Settings.Platform.TIE:
							new TieForm(config, config.RecentMissions[1]).Show();
							break;
						case Settings.Platform.XvT:
							new XvtForm(config, config.RecentMissions[1]).Show();
							break;
						case Settings.Platform.BoP:
							new XvtForm(config, config.RecentMissions[1]).Show();
							break;
						case Settings.Platform.XWA:
							new XwaForm(config, config.RecentMissions[1]).Show();
							break;
					}
				}
				else
				{
					MessageBox.Show("Last Mission value not set, taking you to last platform", "Error");
					switch (config.LastPlatform)
					{
						case Settings.Platform.TIE:
							new TieForm(config).Show();
							break;
						case Settings.Platform.XvT:
							new XvtForm(config, false).Show();
							break;
						case Settings.Platform.BoP:
							new XvtForm(config, true).Show();
							break;
						case Settings.Platform.XWA:
							new XwaForm(config).Show();
							break;
					}
				}
			}
			else	//else process Args normally
			{
				if (Args[0] == "/?")
				{
					MessageBox.Show("Available arguments:\n\tnone\t\t\tStart normally\n\t/?\t\tShows this help\n\t[path]\t\tLoads mission as declared by [path]\n\t/TIE\t\tLoads TIE config directly\n\t/XvT\t\tLoads XvT config directly\n\t/BoP\t\tLoads BoP config directly\n\t/XWA\t\tLoads XWA config directly", "Program use");
					return;
				}
				else if (Args[0].ToUpper() == "/TIE") new TieForm(config).Show();
				else if (Args[0].ToUpper() == "/XVT") new XvtForm(config, false).Show();
				else if (Args[0].ToUpper() == "/BOP") new XvtForm(config, true).Show();
				else if (Args[0].ToUpper() == "/XWA") new XwaForm(config).Show();
				else
				{
					try
					{
						FileStream fsPlat = File.OpenRead(Args[0]);
						short t = new BinaryReader(fsPlat).ReadInt16();
						fsPlat.Close();
						System.Diagnostics.Debug.WriteLine("about to fire");
						switch (t)
						{
							case 12:
								new XvtForm(config, Args[0]).Show();
								break;
							case 14:
								new XvtForm(config, Args[0]).Show();
								break;
							case 18:
								new XwaForm(config, Args[0]).Show();
								break;
							case -1:
								new TieForm(config, Args[0]).Show();
								break;
							default:
								throw new Exception("File is either invalid or corrupted.\nPlease ensure the correct file was selected.");
						}
						System.Diagnostics.Debug.WriteLine("fired");
					}
					catch (Exception e)
					{
						MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Application.Run(new StartForm(config));
					}
				}
			}
			Application.Run();
		}
	}
}
