import "./App.css";
import NavBar from "./Components/NavBar";
import imagePath from "./assets/logo/Codopark.png";

function App() {
  let items =["Home", "Product", "Services"];
  return (
    <div>
      <NavBar brandName="ROLES-MODULES" imageSrcPath={imagePath} navItems={items} />
    </div>
  );
}

export default App;
