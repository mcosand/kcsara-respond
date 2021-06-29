import React from 'react';
import { BrowserRouter as Router, Route } from 'react-router-dom';

import logo from './logo.svg';
import './App.css';
import { MainStore } from './store/main-store';
import { ActivityList } from './screens/ActivityList';
import { observer } from 'mobx-react';

function App(props: {store:MainStore}) {
  const { store } = props;

  return (
    <Router>
      <Route exact path="/">
        <div className="App">
          <ActivityList activities={store.activities} />
          <header className="App-header">
            <img src={logo} className="App-logo" alt="logo" />
            <p>
              Edit <code>src/App.tsx</code> and save to reload.
            </p>
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

  );
}

export default observer(App);
