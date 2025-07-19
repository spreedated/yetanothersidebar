using com.lightstreamer.client;
using Services.Models.Responses;
using System;
using System.Globalization;

namespace Services.Models
{
    public class IssListener : SubscriptionListener
    {
        public IssResponse IssResponse { get; private set; }
        public DateTime Updated { get; private set; }

        public event EventHandler ValuesUpdated;

        public void onClearSnapshot(string itemName, int itemPos)
        {
            //noop
        }

        public void onCommandSecondLevelItemLostUpdates(int lostUpdates, string key)
        {
            //noop
        }

        public void onCommandSecondLevelSubscriptionError(int code, string message, string key)
        {
            //noop
        }

        public void onEndOfSnapshot(string itemName, int itemPos)
        {
            //noop;
        }

        public void onItemLostUpdates(string itemName, int itemPos, int lostUpdates)
        {
            //noop
        }

        public void onItemUpdate(ItemUpdate itemUpdate)
        {
            this.IssResponse ??= new();

            if (itemUpdate.ItemName == "NODE3000005" && float.TryParse(itemUpdate.getValue("Value"), CultureInfo.InvariantCulture, out float val))
            {
                this.IssResponse.UrineTankQuantity = val;
            }

            if (itemUpdate.ItemName == "NODE3000008" && float.TryParse(itemUpdate.getValue("Value"), CultureInfo.InvariantCulture, out float valw))
            {
                this.IssResponse.WasteWaterTankQuantity = valw;
            }

            if (itemUpdate.ItemName == "NODE3000009" && float.TryParse(itemUpdate.getValue("Value"), CultureInfo.InvariantCulture, out float valc))
            {
                this.IssResponse.CleanWaterTankQuantity = valc;
            }

            this.Updated = DateTime.Now;

            this.ValuesUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void onListenEnd()
        {
            throw new NotImplementedException();
        }

        public void onListenStart()
        {
            //noop
        }

        public void onRealMaxFrequency(string frequency)
        {
            //noop
        }

        public void onSubscription()
        {
            //noop
        }

        public void onSubscriptionError(int code, string message)
        {
            //noop
        }

        public void onUnsubscription()
        {
            //noop
        }
    }
}
