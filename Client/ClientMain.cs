using System;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using ScaleformUI;

namespace Bus69.Client
{
    public class ClientMain : BaseScript
    {
        private readonly int playerBlipId = API.GetMainPlayerBlipId();

        public ClientMain()
        {
            Debug.WriteLine("Hi from Bus69.Client!");

            API.SetBlipDisplay(playerBlipId, 8);

            _ = WaitForPlayerToCallBus();
        }

        private async Task WaitForPlayerToCallBus()
        {
            while (API.GetDistanceBetweenCoords(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, -1038.834f, -2733.346f, 19.16927f, false) > 1.0f)
            {
                await Delay(0);
            }

            Notifications.ShowHelpNotification("Press ~INPUT_CONTEXT~ to make bus call.");
            while (!API.IsControlJustPressed(0, (int)Control.Context))
            {
                await Delay(0);
            }

            await CreateBus();
        }

        private async Task CreateBus()
        {
            Ped airdriver = await World.CreatePed(new Model("mp_m_freemode_01"), new Vector3(-1067.831f, -2559.146f, 21.0f), 237.912f);
            API.SetPedComponentVariation(airdriver.Handle, 2, 4, 4, 0); // Hair
            API.SetPedComponentVariation(airdriver.Handle, 7, 38, 0, 0); // Accessory
            API.SetPedComponentVariation(airdriver.Handle, 11, 242, 1, 0); // Jacket
            API.SetPedComponentVariation(airdriver.Handle, 8, 15, 0, 0); // Shirt
            API.SetPedComponentVariation(airdriver.Handle, 4, 35, 0, 0); // Pants
            API.SetPedComponentVariation(airdriver.Handle, 6, 21, 0, 0); // Shoes
            airdriver.BlockPermanentEvents = true;

            Vehicle airbus = await World.CreateVehicle(new Model("airbus"), new Vector3(-1062.101f, -2555.909f, 20.07566f), 150.1505f);
            airbus.Mods.Livery = 1;

            Blip airbusBlip;
            airbusBlip = airbus.AttachBlip();
            airbusBlip.Sprite = (BlipSprite)513;
            airbusBlip.Scale = 0.6f;
            airbusBlip.IsFlashing = true;

            airdriver.Task.WarpIntoVehicle(airbus, VehicleSeat.Driver);
            airdriver.Task.DriveTo(airbus, new Vector3(-1050.137f, -2720.53f, 20.08276f), 1.0f, 10.0f, 786599);

            Notifications.ShowHelpNotification("~BLIP_BUS~ Bus69 is on its way to the station.");

            await Delay(3000);

            airbusBlip.IsFlashing = false;

            Blip blip = World.CreateBlip(new Vector3(-1045.806f, -2722.468f, 20.10379f), 5.0f);
            blip.Color = BlipColor.Yellow;
            blip.IsFlashing = true;

            await Delay(10000);

            await WaitForBusToArrive(airbus);
            await WaitForPlayerToEnterBus(airbus);

            API.SetBlipDisplay(playerBlipId, 0);
            API.SetNewWaypoint(298.8799f, -1202.714f);

            airdriver.Task.DriveTo(airbus, new Vector3(259.2122f, -1179.586f, 29.44427f), 1.0f, 20.0f, 786599);
            blip.IsFlashing = false;
        }

        private async Task WaitForBusToArrive(Vehicle airbus)
        {
            while (API.GetDistanceBetweenCoords(airbus.Position.X, airbus.Position.Y, airbus.Position.Z, -1050.137f, -2720.53f, 20.08276f, false) > 15.0f)
            {
                await Delay(1000);
            }

            airbus.Position = new Vector3(-1050.137f, -2720.53f, 20.08276f);
            airbus.Heading = 238.8681f;

            Notifications.ShowHelpNotification("~BLIP_BUS~ Bus69 has arrived. Jump in!");
        }

        private async Task WaitForPlayerToEnterBus(Vehicle airbus)
        {
            while (API.GetDistanceBetweenCoords(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, -1041.774f, -2726.878f, 20.01925f, false) > 10.0f)
            {
                await Delay(0);
            }

            while (!API.IsControlJustPressed(0, (int)Control.Enter))
            {
                await Delay(0);
            }

            Game.PlayerPed.Task.EnterVehicle(airbus, VehicleSeat.Passenger);

            await Delay(500);

            API.SetFollowPedCamViewMode(4);

            await Delay(6000);

            API.SetFollowVehicleCamViewMode(4);

            Notifications.ShowHelpNotification("~BLIP_BUS~ Bus69 is departuring.");
        }

        [Tick]
        public Task DrawMarker()
        {
            World.DrawMarker(MarkerType.VerticalCylinder, new Vector3(-1038.834f, -2733.346f, 19.16927f), Vector3.Zero, Vector3.Zero, new Vector3(1.0f, 1.0f, 1.0f), Color.FromArgb(255, 0, 255, 0), false, false, false, null, null, false);

            return Task.FromResult(0);
        }
    }
}