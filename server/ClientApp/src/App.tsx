import React from 'react';
import { BrowserRouter as Router, Route } from 'react-router-dom';
import { observer } from 'mobx-react';

import logo from './logo.svg';
import './App.css';
import { MainStore } from './store/main-store';
import { ActivityList } from './screens/ActivityList';
import { SplashScreen } from './screens/Splash';

function App(props: {store:MainStore}) {
  const { store } = props;

  return (!store.user
    ? <SplashScreen isLoadingUser={store.loadingUser} />
    : <Router>
      <Route exact path="/">
        <div className="App">
          <header className="App-header">
            <img src={logo} className="App-logo" alt="logo" />
            <p>
              Edit <code>src/App.tsx</code> and save to reload.
            </p>
            <p>Status: <span>{store.isOnline ? '' : 'Not '} Connected</span> {store.isConnecting ? <span>Connecting ...</span> : undefined}</p>
            <p>User: {store.user
            ? <span>{store.user.name} ({store.user.email})</span>
            : <span>Not logged in <a href="/account/login">Login</a></span>
            }</p>
            <a
              className="App-link"
              href="https://reactjs.org"
              target="_blank"
              rel="noopener noreferrer"
            >
              Learn React
            </a>
          </header>
        </div>
      </Route>
      <Route exact path="/activity"><ActivityList activities={store.activities} /></Route>
    </Router>
  )
}

export default observer(App);
