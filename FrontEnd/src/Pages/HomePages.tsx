import React, { useEffect, useState } from 'react';
import { Button } from '@chakra-ui/react';

const HomePages = () => {
  const [isLoading, setIsLoading] = useState(true);
  
  const [state, setState] = useState<string>("");
  const [deleteCount, setDeleteCount] = useState(0);
  const [editCount, setEditCount] = useState(0);

  useEffect(() => {
    const timeout = setTimeout(() => {
      setState("This is API data");
      setIsLoading(false);
    }, 5000);

    return () => clearTimeout(timeout);
  }, []);

  const handleDelete = () => {
    setIsLoading(true);
    setDeleteCount(deleteCount + 1);
    
    // Simulate an API call or processing time
    const timeout = setTimeout(() => {
      setState(`Data deleted ${deleteCount + 1} times`);
      setIsLoading(false);
    }, 2000);

    return () => clearTimeout(timeout);
  };

  const handleEdit = () => {
    setIsLoading(true);
    setEditCount(editCount + 1);
    
    // Simulate an API call or processing time
    const timeout = setTimeout(() => {
      setState(`Data edited ${editCount + 1} times`);
      setIsLoading(false);
    }, 2000);

    return () => clearTimeout(timeout);
  };

  return (
    <div style={{ marginLeft: "100px" }}>
      {isLoading ? "loading..." : state}
      <p>This is home pages</p>
      <Button colorScheme='red' onClick={handleDelete}>Delete</Button>
      <Button colorScheme='green' onClick={handleEdit}>Edit</Button>
    </div>
  );
};

export default HomePages;
