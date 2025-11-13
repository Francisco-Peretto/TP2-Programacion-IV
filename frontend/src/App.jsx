import React from "react";
import AppRouter from "./router";
import Layout from "./components/Layout";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

export default function App() {
  // ensure dark class on root
  if (typeof document !== 'undefined' && !document.documentElement.classList.contains('dark')) {
    document.documentElement.classList.add('dark');
  }

  return (
    <Layout>
      <AppRouter />
      <ToastContainer position="top-right" autoClose={3000} newestOnTop theme="colored" />
    </Layout>
  );
}
