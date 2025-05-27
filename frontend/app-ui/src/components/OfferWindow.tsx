import "./ClassifiedTemplate.css";

export default function OfferWindow() {
  return (
    <div className="single_ads_card mt-30 border">
      <div className="ads_card_image">
        <img src="..\src\assets\images\ads-1.png" alt="ads" />
      </div>
      <div className="ads_card_content">
        <div className="meta d-flex justify-content-between">
          <p>Frec menta</p>
        </div>
        <h4 className="title">Stima si respect</h4>
        <p>
          <i className="far fa-map-marker-alt"></i>Cluj-Napoca, Cluj
        </p>
        <div className="ads_price_date d-flex justify-content-between">
          <span className="price">2 RON</span>
          <span className="date">21 Apr, 2025</span>
        </div>
      </div>
    </div>
  );
}
