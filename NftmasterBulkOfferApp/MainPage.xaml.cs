using NFTMaster.BulkOfferApp.Model;
using System.Net.WebSockets;
using System.Net;
using System.Text;

namespace NftmasterBulkOfferApp;

public partial class MainPage : ContentPage
{
    public BulkIOUAllocationOfferBatchReturn Batch { get; set; }

    public MainPage()
	{
		InitializeComponent();
	}

    private async void OnSubmitBatchCodeClicked(object sender, EventArgs e)
    {

        Batch = await GetBatch(BulkMintCodeText.Text);

        if (Batch != null && Batch.Offers != null)
        {
            FormResultTxt.IsVisible = true;
            FormResultTxt.Text = "Batch Id: " + BulkMintCodeText.Text + "      Offer Count: " + Batch.Offers.Count().ToString();
            MintView();
        }
        else
        {
            FormResultTxt.IsVisible = true;
            FormResultTxt.Text = "No offers found. " + Batch.Message;
        }
    }



    private async void OnMintBtnClicked(object sender, EventArgs e)
    {
        ProgressView();

        TitleLbl.Text = "We are signing your offers. Don't close the application!";
        ProgressBar.IsVisible = true;

        if (Batch != null && Batch.Offers != null)
        {

            int i = 0;

            foreach (var offer in Batch.Offers)
            {
                System.Threading.Thread.Sleep(2000);
                await SignOffer(offer, XrpAddressTxt.Text, SecretTxt.Text);
                i++;
                ProgressBar.Progress = i / Batch.Offers.Count();
            }
            BatchExecuted(Batch.Id);
            ProgressBar.IsVisible = false;
            TitleLbl.Text = "Batch finished";
        }
    }

    public async Task<dynamic> GetBatch(string id)
    {
        using (var httpClient = new HttpClient())
        {
            string url = "https://api.nftmaster.com/IOUOffer?id=" + id;
            var response = await httpClient.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string batchResponse = response.Content.ReadAsStringAsync().Result;
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<BulkIOUAllocationOfferBatchReturn>(batchResponse);
                }
                catch (Exception e) { }
            }
            return null;
        }
    }

    public async Task BatchExecuted(string id)
    {
        using (var httpClient = new HttpClient())
        {
            string url = "https://api.nftmaster.com/IOUOffer?id=" + id;

            var response = await httpClient.PostAsync(url, null);
        }
    }

    public async Task SignOffer(IOUAllocationOffer offer, string address, string secret)
    {
        var client = new ClientWebSocket();
        await client.ConnectAsync(new Uri("wss://xls20-sandbox.rippletest.net:51233"), CancellationToken.None);
        var sending = Task.Run(async () =>
        {
            string jsonText = "{\"command\":\"submit\",\"secret\":\"" + secret + "\",\"tx_json\":{\"TransactionType\":\"NFTokenCreateOffer\",\"Account\":\"" + address + "\",\"NFTokenID\":\"" + offer.NFTokenID + "\",\"Destination\":\"" + offer.Destination + "\",\"Amount\":\"" + offer.Amount + "\",\"Flags\":" + offer.Flags + ",\"Memos\":[{\"Memo\":{\"MemoType\":\"" + offer.MemoType + "\",\"MemoData\":\"" + offer.MemoData + "\"}}]}}";
            var bytes = Encoding.UTF8.GetBytes(jsonText);
            await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        });
        var s = client;
    }
    private void RestartBtnClicked(object sender, EventArgs e)
    {
        TitleLbl.Text = "NFTMASTER - Bulk Offers";
        BulkMintCodeText.Text = "";
        BatchView();
    }

    private void MintView()
    {
        XrpAddressLbl.IsVisible = true;
        XrpAddressTxt.IsVisible = true;
        SecretLbl.IsVisible = true;
        SecretTxt.IsVisible = true;
        MintBtn.IsVisible = true;
        RestartBtn.IsVisible = true;

        SubmitBatchCodeBtn.IsVisible = false;
        BulkMintCodeText.IsVisible = false;
        BulkMintCodeLbl.IsVisible = false;
    }

    private void BatchView()
    {
        XrpAddressLbl.IsVisible = false;
        XrpAddressTxt.IsVisible = false;
        SecretLbl.IsVisible = false;
        SecretTxt.IsVisible = false;
        MintBtn.IsVisible = false;
        FormResultTxt.IsVisible = false;
        RestartBtn.IsVisible = false;

        SubmitBatchCodeBtn.IsVisible = true;
        BulkMintCodeText.IsVisible = true;
        BulkMintCodeLbl.IsVisible = true;
    }

    private void ProgressView()
    {
        XrpAddressLbl.IsVisible = false;
        XrpAddressTxt.IsVisible = false;
        SecretLbl.IsVisible = false;
        SecretTxt.IsVisible = false;
        MintBtn.IsVisible = false;
    }
}

