namespace IntermediaryTransactionsApp.State
{
    public enum OrderState
    {
        ReadyForTransaction = 1,        
        Canceled = 2,                   
        BuyerInspecting = 3,            
        Completed = 4,                  
        BuyerComplained = 5,           
        SellerMarkedComplaintInvalid = 6, 
        AdminMediationRequested = 7,    
        AwaitingBuyerConfirmation = 8
    }
}
