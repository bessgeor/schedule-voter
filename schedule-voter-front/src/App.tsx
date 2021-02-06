import React from 'react'
import * as moment from 'moment'
import { parse } from 'query-string'
import { Form } from './voting/Form'
import { Vote, Votes } from './voting/types'
import { VoteResultsDisplay } from './voting/VoteResultsDisplay'

moment.locale('ru')

function* generateDates(start: moment.Moment, weeks: number) {
  for (let i = 0; i < weeks*7; i++)
    for (let h = 0; h < 24; h += 3)
      yield moment.utc(start).add(i, 'days').add(h, 'hours')
}

function App() {
  const qs = React.useMemo(() => parse(window.location.search), [parse])
  const { staticName, startDate, weeks } = React.useMemo(() => ({
    staticName: String(qs['staticName']),
    startDate: moment.utc(qs['startDate']),
    weeks: Number(qs['weeks'])
  }), [qs])
  const dates = React.useMemo(() => Array.from(generateDates(startDate, weeks)), [startDate, weeks])

  const lsKey = React.useMemo(() => "voted-" + staticName + startDate.format("-YYYY-MM-DD-") + weeks, [staticName, startDate, weeks])

  const [voted, setVotedState] = React.useState(() => window.localStorage.getItem(lsKey) !== null)
  const setVoted = React.useCallback(() => {
    setVotedState(true)
    window.localStorage.setItem(lsKey, 'true')
  }, [setVotedState, lsKey])
  const [showVoteResults, setShowVoteResults] = React.useState(voted)
  const goResults = React.useCallback(() => setShowVoteResults(true), [setShowVoteResults])
  const goVoting = React.useCallback(() => setShowVoteResults(false), [setShowVoteResults])

	const [gw2User, setGw2User] = React.useState('')
	const [disUser, setDisUser] = React.useState('')

	const [votes, setVotes] = React.useState(() => dates.map(tourney => ({ tourney, vote: Vote.CantAttend })))

  const [voteResult, setVoteResult] = React.useState<Votes | null>(null)
  const onApprove = React.useCallback((votes: Votes) => setVoteResult(votes), [setVoteResult])
  const resetVotes = React.useCallback(() => setVoteResult(null), [setVoteResult])

  React.useEffect(() => {
    if (voteResult)
    {
      const aborter = new AbortController()
      fetch('/approve', {
        method: 'PUT',
        signal: aborter.signal,
        body: JSON.stringify(voteResult),
        headers: {
          ['content-type']: 'application/json'
        }
      }).then(setVoted).then(resetVotes).catch(resetVotes)
      return () => aborter.abort()
    }
  }, [voteResult])

  return (
    showVoteResults
    ? <VoteResultsDisplay
      staticName={staticName}
      dates={dates}
      voted={voted}
      goVote={goVoting}
    />
    : <Form
      isLoading={voteResult != null}
      viewResults={goResults}
      dates={dates}
      staticName={staticName}
      onApprove={onApprove}
      votes={votes}
      setVotes={setVotes}
      gw2User={gw2User}
      setGw2User={setGw2User}
      disUser={disUser}
      setDisUser={setDisUser}
    />
  );
}

export default App;
