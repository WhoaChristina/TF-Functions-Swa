import './App.css';
import React, { useState, useEffect } from 'react';

function App() {

  const [foodDrinkData, setFoodDrinkData] = useState([])

  useEffect(() => {
    getFoodDrinkData().then(data => {
        console.log(data)
        setFoodDrinkData(data)
     });
  }, [])

  async function getFoodDrinkData() {

    const response = await fetch("api/FoodDrinkApi");
    return await response.json();
}
  return (
    <div className='App'>
      <h1>Something edible and something to drink</h1>
      <div className='Food'>
      <p>{foodDrinkData.foodName}</p>
      <img src={foodDrinkData.foodImg}/>
      </div>
      <div className='Drink'>
      <p>{foodDrinkData.drinkName}</p>
      <img src={foodDrinkData.drinkImg}/>
      </div>
    </div>
  );
}
export default App;