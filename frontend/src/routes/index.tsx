import React from 'react';

export const LandingPage: React.FC = () => {
  return (
    <div style={{ textAlign: 'center', padding: '50px' }}>
      <h1>Welcome to Our Website</h1>
      <p>Your journey to excellence starts here.</p>
      <button onClick={() => alert('Get Started!')}>Get Started</button>
    </div>
  );
};
