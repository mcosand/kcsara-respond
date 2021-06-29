import React from 'react';

export const SplashScreen :React.FC<{ isLoadingUser: boolean }> = ({ isLoadingUser }) => {
  return (
    isLoadingUser
    ? <div>Loading ...</div>
    : <div>Not logged in. <a href="/account/login">Login</a></div>
  );
}

export default SplashScreen