import { useState, useEffect } from "react";
import { Product } from "../../app/models/product";
import ProductList from "./ProductList";
import agent from "../../app/api/agent";
import LoadingComponent from "../../app/layout/LoadingComponent";

export default function Catalog() {
  const [products, setProducts] = useState<Product[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    agent.Catalog.list()
      .then(data => {
        console.log('API response:', data); // Log the response

        if (data.items && Array.isArray(data.items)) {
          setProducts(data.items);
        } else {
          throw new Error('API response does not contain a valid items array');
        }
      })
      .catch(error => {
        console.error('There was a problem with the fetch operation:', error);
        setError(error.message);
      })
      .finally(() => setLoading(false))
      ;
  }, []);

  if (loading) return <LoadingComponent/>

  return (
    <>
      <ProductList products={products} />
      {error && <div>Error: {error}</div>}
    </>

  )
}