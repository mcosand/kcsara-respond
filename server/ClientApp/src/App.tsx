import React from 'react';
import { Router, Route, Redirect } from 'react-router-dom';
import { observer } from 'mobx-react';
import { unstable_createMuiStrictModeTheme as createTheme } from '@material-ui/core';
import AdapterLuxon from '@material-ui/lab/AdapterLuxon';
import LocalizationProvider from '@material-ui/lab/LocalizationProvider';
import './App.css';
import { MainStore } from './store/main-store';
import ActivityList from './screens/activities/ActivityList';
import { SplashScreen } from './screens/Splash';
import ActivityCreate from './screens/activities/ActivityCreate';
import MyStatus from './screens/me/MyStatus';

import { ThemeProvider } from '@material-ui/core';
import { Page } from './components/Page';

const InsideStore :React.FC<{ store: MainStore }> = ({store}) => {
  const theme = createTheme({
    palette: {
      primary: { main: store.branding.color ?? '#888' }
    }
  });

  React.useEffect(() => {
    document.title = `${store.branding?.name ?? 'KCSARA'} Respond`;
  }, [store.branding?.name]);

  return (<LocalizationProvider dateAdapter={AdapterLuxon}>
    <ThemeProvider theme={theme}>
      {!store.user.obj
        ? <Router history={store.history}><Page><SplashScreen isLoadingUser={store.user.loading} /></Page></Router>
        : <Router history={store.history}>
          <Route exact path="/"><Redirect to="/me" /></Route>
          <Route exact path="/me"><MyStatus store={store} /></Route>
          <Route exact path="/activity"><ActivityList store={store} /></Route>
          <Route exact path="/activity/create"><ActivityCreate store={store} /></Route>
          {/* <Page store={store}><div className="App">
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
              <p><Link to="/admin/locations">Location Admin</Link></p>
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
          </Page> */}
        </Router>
      }
    </ThemeProvider>
  </LocalizationProvider>
  )
}

const ObserverApp = observer(InsideStore);

export default function App(props: { store: MainStore }) {
  const { store } = props;

  React.useEffect(() => {
    store.registerRoutes([
      { path: '/me', name: store.user.obj?.name ?? 'User' },
      { path: '/activity', name: 'Activity' },
      { path: '/activity/create', name: 'New Activity' },
    ]);
  }, [store]);

  return <ObserverApp store={store} />;
}
