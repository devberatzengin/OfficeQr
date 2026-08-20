import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./auth/AuthContext";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { Layout } from "./components/Layout";
import { LoginPage } from "./pages/LoginPage";
import { RegisterPage } from "./pages/RegisterPage";
import { HomePage } from "./pages/HomePage";
import { ScanPage } from "./pages/ScanPage";
import { CabinetsListPage } from "./pages/CabinetsListPage";
import { CabinetDetailPage } from "./pages/CabinetDetailPage";
import { CabinetCreatePage } from "./pages/CabinetCreatePage";
import { ShelfDetailPage } from "./pages/ShelfDetailPage";
import { ShelfCreatePage } from "./pages/ShelfCreatePage";
import { ItemsListPage } from "./pages/ItemsListPage";
import { ItemDetailPage } from "./pages/ItemDetailPage";
import { ItemCreatePage } from "./pages/ItemCreatePage";
import { MyActivityPage } from "./pages/MyActivityPage";
import { NotFoundPage } from "./pages/NotFoundPage";

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          <Route element={<ProtectedRoute />}>
            <Route element={<Layout />}>
              <Route path="/" element={<HomePage />} />
              <Route path="/scan" element={<ScanPage />} />

              <Route path="/cabinets" element={<CabinetsListPage />} />
              <Route path="/cabinets/new" element={<CabinetCreatePage />} />
              <Route path="/cabinets/:id" element={<CabinetDetailPage />} />

              <Route path="/shelves/new" element={<ShelfCreatePage />} />
              <Route path="/shelves/:id" element={<ShelfDetailPage />} />

              <Route path="/items" element={<ItemsListPage />} />
              <Route path="/items/new" element={<ItemCreatePage />} />
              <Route path="/items/:id" element={<ItemDetailPage />} />

              <Route path="/my-activity" element={<MyActivityPage />} />

              <Route path="*" element={<NotFoundPage />} />
            </Route>
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}
