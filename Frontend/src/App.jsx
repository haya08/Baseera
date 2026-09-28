import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";

function Dashboard() {
  return <h1>Dashboard</h1>;
}

function DataSources() {
  return <h1>Data Sources</h1>;
}

function CollectedData() {
  return <h1>Collected Data</h1>;
}

function KnowledgeExtraction() {
  return <h1>Knowledge Extraction</h1>;
}

function KnowledgeGraph() {
  return <h1>Knowledge Graph</h1>;
}

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<MainLayout />}>
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/data-sources" element={<DataSources />} />
          <Route path="/collected-data" element={<CollectedData />} />
          <Route
            path="/knowledge-extraction"
            element={<KnowledgeExtraction />}
          />
          <Route path="/knowledge-graph" element={<KnowledgeGraph />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;