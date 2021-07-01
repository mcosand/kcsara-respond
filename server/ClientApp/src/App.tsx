import React from 'react';
import { BrowserRouter as Router, Route } from 'react-router-dom';
import { observer } from 'mobx-react';
import { unstable_createMuiStrictModeTheme as createTheme } from '@material-ui/core';
import AdapterLuxon from '@material-ui/lab/AdapterLuxon';
import LocalizationProvider from '@material-ui/lab/LocalizationProvider';
import logo from './logo.svg';
import './App.css';
import { MainStore } from './store/main-store';
import { ActivityList } from './screens/ActivityList';
import { SplashScreen } from './screens/Splash';
import ActivityCreate from './screens/ActivityCreate';
import { ThemeProvider } from '@material-ui/core';

function App(props: {store:MainStore}) {
  const { store } = props;

  const theme = createTheme({});

  return (!store.user.obj
    ? <SplashScreen isLoadingUser={store.user.loading} />
    : <LocalizationProvider dateAdapter={AdapterLuxon}>
      <ThemeProvider theme={theme}>
        <Router>
          <Route exact path="/">
            <div className="App">
              <header className="App-header">
                <img src={logo} className="App-logo" alt="logo" />
                <p>
                  Edit <code>src/App.tsx</code> and save to reload.
                </p>
                <p>Status: <span>{store.isOnline ? '' : 'Not '} Connected</span> {store.isConnecting ? <span>Connecting ...</span> : undefined}</p>
                <p>User: {store.user.obj
                ? <span>{store.user.obj.name} ({store.user.obj.email})</span>
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
          <Route exact path="/activity/create"><ActivityCreate store={store} /></Route>
        </Router>
      </ThemeProvider>
    </LocalizationProvider>
  )
}

export default observer(App);
